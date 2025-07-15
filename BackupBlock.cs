using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace BackupBlock
{
    readonly struct FileTypesPatterns
    {
        public static List<string> protocol_file_type = ["EIAS", "Simple"];

        public static Dictionary<string, string> file_patterns = new()
        {
            [protocol_file_type[0]] = "^\\d{5}-\\d{2}-\\d{2}-",
            [protocol_file_type[1]] = "^\\d{1,4}-(ф|фа|р|ра|м|ма)-"
        };

        public static Dictionary<string, string> file_types = new()
        {
            ["PDF"] = "pdf"
        };
    }


    readonly struct MonthValues
    {
        public static List<string> Month_Names { get; } = ["Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"];
        public static Dictionary<string, int> Table { get; set; } = [];
        static MonthValues()
        {
            for (int value_index = 0; value_index < Month_Names.Count; value_index++)
            {
                Table.Add(Month_Names[value_index], value_index + 1);
            }
        }
    }              
    

    class ProtocolScanPattern(int month_value)
    {
        private static readonly DateTime current_date = DateTime.Now;
        private static readonly string file_type = FileTypesPatterns.file_types["PDF"];
                
        public Func<string, Regex> CreatePattern = (file_pattern) => new(string.Concat(file_pattern, "\\d{2}\\.", $"0{month_value}", "\\.", current_date.Year.ToString(), "\\.", file_type, "$"), RegexOptions.IgnoreCase);      
    }
        

    abstract class BackupFileType 
    {
        private readonly FileInfo[] files;

        public BackupFileType(string file_type, string drive_directory)
        {
            DirectoryInfo directory = new(drive_directory);
            files = directory.GetFiles($"*.{file_type}");
        }

        public FileInfo[]? Files
        { 
            get
            {
                if (files.Length is not 0)
                {
                    return files;
                }
                else
                {
                    return null;
                }
            }
        }

        public abstract List<FileInfo>? GrabMatchedFiles(Regex pattern);
    }
                   

    class PdfFiles(string file_type, string drive_directory) : BackupFileType(file_type, drive_directory) 
    {
        public override List<FileInfo>? GrabMatchedFiles(Regex pattern)
        {
            IEnumerable<FileInfo> backup_block = from file in Files
                                                 where pattern.IsMatch(file.Name)
                                                 select file;
            
            if (backup_block.Any())
            {
                return [.. backup_block];
            }
            else
            {
                return null;
            }
        }      
    }

    
    struct SimpleProtocolTypes
    {
        public static List<string> protocol_types = ["ф", "фа", "р", "ра", "м", "ма"];
        public static int types_count = protocol_types.Count;
    }


    class ProtocolTypeNumbers 
    {
        private const string number_capture_pattern = "^(?<number>\\d+)-";
        private static readonly Regex number_capture = new(number_capture_pattern, RegexOptions.Compiled);
        private static readonly Func<string, Regex> ProtocolTypeCapture = (type) => new($"{number_capture_pattern}{type}-", RegexOptions.IgnoreCase);
        public List<List<int>?> Numbers { get; private set; } = [];

        public ProtocolTypeNumbers(List<FileInfo> backup_files)
        {
            for (int type_index = 0; type_index < SimpleProtocolTypes.types_count; type_index++)
            {
                IEnumerable<string> type_block = from file in backup_files
                                                 where ProtocolTypeCapture(SimpleProtocolTypes.protocol_types[type_index]).IsMatch(file.Name)
                                                 select file.Name;
                
                List<string> current_protocols = [.. type_block];

                if (current_protocols.Count > 0)
                {
                    Numbers.InsertRange(type_index, ConvertToNumbers(current_protocols));
                }
                else
                {
                    List<int>? none_protocols = null;
                    Numbers.InsertRange(type_index, none_protocols);
                }
            }
        }
                
        private static List<int> ConvertToNumbers(List<string> protocol_type_list)
        {
            List<int> numbers = [];

            foreach (string protocol in protocol_type_list)              // если один протокол ??
            {
                Match match = number_capture.Match(protocol);

                if (match.Success)
                {
                    numbers.Add(Convert.ToInt32(match.Groups["number"].Value));
                }
                else 
                { 
                    Console.WriteLine("\nОшибка захвата номера протокола !"); 
                }
            }
            return numbers;
        }        
    }


    abstract class ExtremeNumbers(List<List<int>?> protocol_type_numbers) 
    {
        private readonly List<int> numbers = [];
        private protected abstract int GetExtremeNumber(List<int> current_numbers);

        public List<int> Numbers
        {
            get
            {
                for (int type_index = 0; type_index < SimpleProtocolTypes.types_count; type_index++)
                {
                    List<int>? current_numbers = protocol_type_numbers[type_index];

                    if (current_numbers is not null)      // если один протокол, то что мин и макс ?
                    {
                        numbers.Insert(type_index, GetExtremeNumber(current_numbers));
                    }
                    else
                    {
                        numbers.Insert(type_index, 0);
                    }
                }
                return numbers;
            }
        }
    }


    class MaximumNumbers(List<List<int>?> protocol_type_numbers) : ExtremeNumbers(protocol_type_numbers)
    {
        private protected override int GetExtremeNumber(List<int> current_numbers)
        {
            return current_numbers.Max();
        }
    }


    class MinimumNumbers(List<List<int>?> protocol_type_numbers) : ExtremeNumbers(protocol_type_numbers)
    {
        private protected override int GetExtremeNumber(List<int> current_numbers)
        {
            return current_numbers.Min();
        }
    }
}

