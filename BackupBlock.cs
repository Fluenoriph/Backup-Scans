using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Tracing;


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
        public static int Month_Count { get; } = Month_Names.Count;
        public static Dictionary<string, int> Table { get; } = [];
        static MonthValues()
        {
            for (int value_index = 0; value_index < Month_Count; value_index++)
            {
                Table.Add(Month_Names[value_index], value_index + 1);
            }
        }
    }              
    

    struct CurrentYear
    {
        public static string Year 
        { 
            get
            {
                DateTime current_date = DateTime.Now;
                return current_date.Year.ToString();
            }
        }
    }
                   

    abstract class BackupFileType 
    {
        private readonly FileInfo[] files;
        private protected abstract string File_Type { get; set; }
        
        public BackupFileType(string drive_directory)
        {
            DirectoryInfo directory = new(drive_directory);
            files = directory.GetFiles($"*.{File_Type}");
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
                   

    class PdfFiles(string drive_directory) : BackupFileType(drive_directory) 
    {
        private protected override string File_Type { get; set; } = FileTypesPatterns.file_types["PDF"];

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
    }

        
    class ProtocolTypeNumbers 
    {
        private const string number_capture_pattern = "^(?<number>\\d+)-";
        private static readonly Regex number_capture = new(number_capture_pattern, RegexOptions.Compiled); // изменить без предупр. /pragma ??
        private static readonly Func<string, Regex> ProtocolTypeCapture = (type) => new($"{number_capture_pattern}{type}-", RegexOptions.IgnoreCase);
        
        public Dictionary<string, List<int>> Numbers { get; } = [];

        public ProtocolTypeNumbers(List<FileInfo> backup_files)
        {
            for (int type_index = 0; type_index < SimpleProtocolTypes.protocol_types.Count; type_index++)
            {
                IEnumerable<string> type_block = from file in backup_files
                                                 where ProtocolTypeCapture(SimpleProtocolTypes.protocol_types[type_index]).IsMatch(file.Name)
                                                 select file.Name;

                List<string> current_protocols = [.. type_block];

                if (current_protocols.Count > 0)
                {
                    Numbers.Add(ProtocolFullTypeLocation.type_location_sums[type_index], ConvertToNumbers(current_protocols));
                }   
            }
        }
                
        private static List<int> ConvertToNumbers(List<string> protocol_type_list)
        {
            List<int> numbers = [];

            foreach (string protocol in protocol_type_list)              
            {
                Match match = number_capture.Match(protocol);

                if (match.Success)
                {
                    numbers.Add(Convert.ToInt32(match.Groups["number"].Value));
                }
                else 
                { 
                    Console.WriteLine("\nОшибка захвата номера протокола !"); // shutdown app ??
                }
            }
            return numbers;
        }        
    }


    abstract class ExtremeNumbers(Dictionary<string, List<int>> protocol_type_numbers) 
    {
        private readonly Dictionary<string, int> numbers = [];
        private protected abstract int GetExtremeNumber(List<int> current_numbers);

        public Dictionary<string, int> Numbers
        {
            get
            {
                foreach (var item in protocol_type_numbers)
                {
                    numbers.Add(item.Key, GetExtremeNumber(item.Value));                                        
                }

                return numbers;
            }
        }
    }


    class MaximumNumbers(Dictionary<string, List<int>> protocol_type_numbers) : ExtremeNumbers(protocol_type_numbers)
    {
        private protected override int GetExtremeNumber(List<int> current_numbers)
        {
            return current_numbers.Max();
        }
    }


    class MinimumNumbers(Dictionary<string, List<int>> protocol_type_numbers) : ExtremeNumbers(protocol_type_numbers)
    {
        private protected override int GetExtremeNumber(List<int> current_numbers)
        {
            return current_numbers.Min();
        }
    }
}

