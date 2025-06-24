using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace BackupBlock
{
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

    struct RgxPattern(string file_pattern, int month_value, string file_type)   
    {
        private static readonly DateTime current_date = DateTime.Now;
        public readonly Regex Full_Pattern
        {
            get => CreateFullPattern();
        }

        public readonly Regex CreateFullPattern()
        {
            string full_pattern = string.Concat(file_pattern, "\\d{2}\\.", $"0{month_value}", "\\.", current_date.Year.ToString(), "\\.", file_type, "$");
            
            return new(full_pattern, RegexOptions.IgnoreCase);  
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

            if (files.Length == 0)
            {
                return null;
            }
            else
            {
                return files;
            }           
        }
    }


    class BackupItem(RgxPattern rgx_pattern, FileInfo[] files_found)
    {
        public List<FileInfo>? Result_Files
        {
            get => GetBackupingItems();
        }
        public bool Search_Status { get; private set; }

        private List<FileInfo>? GetBackupingItems()
        {
            IEnumerable<FileInfo> backup_block = from file in files_found
                                                  where rgx_pattern.Full_Pattern.IsMatch(file.Name)
                                                  select file;

            List<FileInfo> result_files = [.. backup_block];

            if (result_files.Count != 0)
            {
                Search_Status = true;

                return result_files;
            }
            else 
            { 
                Search_Status = false;
                
                return null;  // test realy null !!!
            }
        }
    }

    // Базовый класс
    class Protocols(List<FileInfo> backup_files)
    {
        private protected static readonly List<string> protocol_types = ["ф", "фа", "р", "ра", "м", "ма"];
        private protected static int types_count = protocol_types.Count;
        private const string number_capture_pattern = "^(?<number>\\d+)-";
        private readonly Func<string, Regex> protocol_type_pattern = (type) => new($"{number_capture_pattern}{type}-", RegexOptions.IgnoreCase);
        
        public List<List<int>> Protocol_Type_Numbers
        {
            get => GetProtocolTypeNumbers();
        }

        private List<List<int>> GetProtocolTypeNumbers() 
        {
            List<List<int>> protocol_type_numbers = new(types_count);

            for (int type_index = 0; type_index < types_count; type_index++)
            {
                IEnumerable<string> block_of_type = from file in backup_files
                                                    where protocol_type_pattern(protocol_types[type_index]).IsMatch(file.Name)
                                                    select file.Name;
                                
                List<string> type_list = [.. block_of_type];            // может быть один протокол !!
                
                if (type_list.Count != 0)
                {
                    List<int> numbers_of_type = GetNumbersAtType(type_list);

                    if (type_list.Count == numbers_of_type.Count)           // дополнительная проверка количества номеров
                    {
                        protocol_type_numbers.InsertRange(type_index, numbers_of_type);
                    }
                    else { Console.WriteLine("\nНеизвестная ошибка (количество не совпадает - 'GetProtocolTypeNames')"); }
                }                    
                else
                {
                    Console.WriteLine($"\nType {protocol_types[type_index]} none");
                    List<int> none_type = [0];
                    protocol_type_numbers.InsertRange(type_index, none_type);
                }                
            }

            return protocol_type_numbers;
        }

        private static List<int> GetNumbersAtType(List<string> protocol_type_list)
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
         
        private protected Dictionary<string, int> GetMaxNumbers()
        {
            ProtocolTypes max_numbers = new();

            for (int type_index = 0; type_index < types_count; type_index++)
            {
                //int min = protocol_type_numbers[type_index].Min();
                max_numbers.Table.Add(protocol_types[type_index], Protocol_Type_Numbers[type_index].Max());      // если 0, то какое будет минимальное значение
            }

            return max_numbers.Table;
        }       
    }

    // если январь то передать null ****no
    class MissingProtocols(List<FileInfo> backup_files, Dictionary<string, int>? max_numbers_previous_period) : Protocols(backup_files)
    {
        private readonly List<string> missing_protocols = [];
        private readonly List<string> unknown_protocols = [];
        private Dictionary<string, int> min_numbers = [];

        public List<string> Missing_Protocols
        {
            get => GetMissingProtocols();
        }
        
        public List<string>? Unknown_Protocols       // если январь то равно нулл
        {
            get => GetUnknownProtocols();
        }

        private Dictionary<string, int> GetMinNumbers()
        {
            ProtocolTypes min_numbers = new();

            for (int type_index = 0; type_index < types_count; type_index++)
            {
                //int min = protocol_type_numbers[type_index].Min();
                min_numbers.Table.Add(protocol_types[type_index], protocol_type_numbers[type_index].Min());      // если 0, то какое будет минимальное значение
            }

            return min_numbers.Table;
        }

        private static List<int> CreateRange(int start, int end)
        {
            List<int> range = [];

            for (int i = start; i < end + 1; i++) range.Add(i);
            
            return range;
        }

        private List<string> GetMissingProtocols()
        {
            min_numbers = GetMinNumbers(); 
            Dictionary<string, int> max_numbers = GetMaxNumbers();

            for (int type_index = 0; type_index < types_count; type_index++)
            {
                string current_type = protocol_types[type_index];
                List<int> current_protocols = protocol_type_numbers[type_index];

                if (current_protocols.Count > 2)
                {
                    List<int> range = CreateRange(min_numbers[current_type], max_numbers[current_type]);

                    List<int> missing_numbers = (List<int>)range.Except(current_protocols);

                    foreach (int number in missing_numbers)
                    {
                        missing_protocols.Add($"{number}-{current_type}");
                    }
                }
            }

            return missing_protocols;
        }

        private List<string>? GetUnknownProtocols()
        {
            if (max_numbers_previous_period != null)
            {
                for (int type_index = 0; type_index < types_count; type_index++)
                {
                    string current_type = protocol_types[type_index];
                    int current_min_number = min_numbers[current_type];
                    int current_max_number = max_numbers_previous_period[current_type];

                    if ((current_max_number < current_min_number) & ((current_min_number - 1) != current_max_number))
                    {
                        for (int start_num = current_max_number + 1; start_num < current_min_number; start_num++)     // если один протокол ??
                        {
                            unknown_protocols.Add($"{start_num}-{current_type}");
                        }
                    }
                }

                return unknown_protocols;
            }
            else
            {
                return null;
            }
        }
    }



}
