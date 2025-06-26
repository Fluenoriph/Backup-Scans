using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace BackupBlock
{
    readonly struct FileTypesPatterns
    {
        public static Dictionary<string, string> File_Patterns { get; } = new()
        {
            ["Simple"] = "^\\d{1,4}-(ф|фа|р|ра|м|ма)-",
            ["EIAS"] = "^\\d{5}-\\d{2}-\\d{2}-"
        };

        public static Dictionary<string, string> File_Types { get; } = new()
        {
            ["PDF"] = "*.pdf"
        };
    }


    readonly struct MonthValues
    {
        public static Dictionary<string, int> Table { get; } = new()
        {
            ["Январь"] = 1,
            ["Февраль"] = 2,
            ["Март"] = 3,
            ["Апрель"] = 4,
            ["Май"] = 5,
            ["Июнь"] = 6,
            ["Июль"] = 7,
            ["Август"] = 8,
            ["Сентябрь"] = 9,
            ["Октябрь"] = 10,
            ["Ноябрь"] = 11,
            ["Декабрь"] = 12
        };

        public MonthValues() { }
    }


    struct ProtocolTypes
    {
        public Dictionary<string, int> Table { get; set; } = new()
        {
            ["ф"] = 0,
            ["фа"] = 0,
            ["р"] = 0,
            ["ра"] = 0,
            ["м"] = 0,
            ["ма"] = 0
        };

        public ProtocolTypes() { }
    }


    interface IRgxPattern
    {
        Regex Full_Pattern { get; }

        Regex CreateFullPattern();
    }


    struct ProtocolScan(string file_pattern, int month_value) : IRgxPattern
    {
        private static readonly DateTime current_date = DateTime.Now;
        private static readonly string file_type = FileTypesPatterns.File_Types["PDF"];
        public readonly Regex Full_Pattern
        {
            get => CreateFullPattern();
        }

        private readonly Regex CreateFullPattern()
        {
            string full_pattern = string.Concat(file_pattern, "\\d{2}\\.", $"0{month_value}", "\\.", current_date.Year.ToString(), "\\.", file_type, "$");
            return new(full_pattern, RegexOptions.IgnoreCase);  
        }

        readonly Regex IRgxPattern.CreateFullPattern()
        {
            return CreateFullPattern();
        }
    }
        

    struct FilesAtType(string file_type, string drive_directory)
    {
        public readonly FileInfo[]? Received_Files 
        { 
            get => GetAllFilesToType(); 
        }

        private readonly FileInfo[]? GetAllFilesToType()         
        {
            DirectoryInfo directory = new(drive_directory);
            FileInfo[] files = directory.GetFiles(file_type);

            if (files.Length != 0)
            {
                return files;
            }
            else
            {
                return null;
            }           
        }
    }


    class BackupItem(Regex pattern, FileInfo[] files_found)
    {
        public int Files_Count { get; private set; }
        public List<FileInfo>? Result_Files 
        { 
            get => GetBackupingItems(); 
        }

        private IEnumerable<FileInfo> GetMatchedItems()
        {
            IEnumerable<FileInfo> backup_block = from file in files_found
                                                 where pattern.IsMatch(file.Name)
                                                 select file;
            return backup_block;
        }

        private List<FileInfo>? GetBackupingItems()
        {
            List<FileInfo> result_files = [.. GetMatchedItems()];

            Files_Count = result_files.Count;

            if (Files_Count != 0)
            {
                return result_files;
            }
            else
            {
                return null;  // test realy null !!!
            }
        }
    }
                      

    interface ISimpleProtocolTypes
    {
        static List<string> protocol_types = ["ф", "фа", "р", "ра", "м", "ма"];
        static int types_count = protocol_types.Count;
    }


    class ProtocolTypeNumbers(List<FileInfo> backup_files) : ISimpleProtocolTypes
    {
        private const string number_capture_pattern = "^(?<number>\\d+)-";
        private readonly Func<string, Regex> ProtocolTypePattern = (type) => new($"{number_capture_pattern}{type}-", RegexOptions.IgnoreCase);
                
        private static List<int> GetNumbersType(List<string> protocol_type_list)
        {
            List<int> numbers = [];

            foreach (string protocol in protocol_type_list)              // если один протокол ??
            {
                Match match = Regex.Match(protocol, number_capture_pattern);

                if (match.Success)
                {
                    numbers.Add(Convert.ToInt32(match.Groups["number"].Value));
                }
                else { Console.WriteLine("\nОшибка захвата номера протокола !"); }
            }
            return numbers;
        }

        public List<List<int>> GetNumbers()
        {
            List<List<int>> protocol_type_numbers = new(ISimpleProtocolTypes.types_count);

            for (int type_index = 0; type_index < ISimpleProtocolTypes.types_count; type_index++)
            {
                IEnumerable<string> type_block = from file in backup_files
                                                    where ProtocolTypePattern(ISimpleProtocolTypes.protocol_types[type_index]).IsMatch(file.Name)
                                                    select file.Name;

                List<string> type_list = [.. type_block];            // может быть один протокол !!

                if (type_list.Count != 0)
                {
                    List<int> numbers_of_type = GetNumbersType(type_list);

                    if (type_list.Count == numbers_of_type.Count)           // дополнительная проверка количества номеров
                    {
                        protocol_type_numbers.InsertRange(type_index, numbers_of_type);
                    }
                    else { Console.WriteLine("\nНеизвестная ошибка (количество не совпадает - 'GetProtocolTypeNames')"); }
                }
                else
                {
                    Console.WriteLine($"\nType {ISimpleProtocolTypes.protocol_types[type_index]} none");
                    List<int> none_type = [0];
                    protocol_type_numbers.InsertRange(type_index, none_type);
                }
            }
            return protocol_type_numbers;
        }
    }


    class ExtremeNumbers(List<List<int>> protocol_type_numbers) : ISimpleProtocolTypes
    {
        public Dictionary<string, int> GetMaxNumbers()
        {
            ProtocolTypes max_numbers = new();

            for (int type_index = 0; type_index < ISimpleProtocolTypes.types_count; type_index++)
            {
                //int min = protocol_type_numbers[type_index].Min();
                max_numbers.Table.Add(ISimpleProtocolTypes.protocol_types[type_index], protocol_type_numbers[type_index].Max());      // если 0, то какое будет минимальное значение
            }
            return max_numbers.Table;
        }

        public Dictionary<string, int> GetMinNumbers()
        {
            ProtocolTypes min_numbers = new();

            for (int type_index = 0; type_index < ISimpleProtocolTypes.types_count; type_index++)
            {
                //int min = protocol_type_numbers[type_index].Min();
                min_numbers.Table.Add(ISimpleProtocolTypes.protocol_types[type_index], protocol_type_numbers[type_index].Min());      // если 0, то какое будет минимальное значение
            }
            return min_numbers.Table;
        }
    }


        
}

