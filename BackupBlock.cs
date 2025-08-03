using System.Text.RegularExpressions;
using Tracing;
using TextData;


namespace BackupBlock
{
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
                        

    class SourceFiles
    {
        private readonly FileInfo[] files;
        public string File_Type { get; set; } = AppConstants.scan_file_type;

        public SourceFiles(string drive_directory)
        {
            DirectoryInfo directory = new(drive_directory);
            files = directory.GetFiles($"*.{File_Type}");  
        }
                
        public List<FileInfo>? GrabMatchedFiles(Regex pattern)
        {
            IEnumerable<FileInfo> backup_block = from file in files
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
               
        
    class ProtocolTypeNumbers 
    {
#pragma warning disable SYSLIB1045
        private static readonly Regex number_capture = new(AppConstants.simple_number_pattern); 
#pragma warning restore SYSLIB1045
        public Dictionary<string, List<int>> Numbers { get; } = [];

        public ProtocolTypeNumbers(Dictionary<string, List<FileInfo>> simple_files)
        {
            foreach (var item in simple_files)
            {
                Numbers.Add(item.Key, ConvertToNumbers(item.Value)); 
            }
        }
                
        private static List<int> ConvertToNumbers(List<FileInfo> protocol_type_list)
        {
            List<int> numbers = [];

            foreach (var protocol in protocol_type_list)              
            {
                Match match = number_capture.Match(protocol.Name);

                if (match.Success)
                {
                    numbers.Add(Convert.ToInt32(match.Groups["number"].Value));
                }
                else 
                { 
                    Console.WriteLine("\nОшибка захвата номера протокола !"); // shutdown app ??
                }
            }

            numbers.Sort();
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
            return current_numbers.Last();
        }
    }


    class MinimumNumbers(Dictionary<string, List<int>> protocol_type_numbers) : ExtremeNumbers(protocol_type_numbers)
    {
        private protected override int GetExtremeNumber(List<int> current_numbers)
        {
            return current_numbers.First();
        }
    }
}

