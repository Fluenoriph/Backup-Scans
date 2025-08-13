using System.Text.RegularExpressions;
using TextData;
using Tracing;
using static System.Runtime.InteropServices.JavaScript.JSType;


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
           
        
    class SimpleProtocolsNumbers
    {
        public Dictionary<string, List<int>> Numbers { get; } = [];

        public SimpleProtocolsNumbers(Dictionary<string, List<FileInfo>> files)
        {
            foreach (var item in files)
            {
                List<int> type_numbers = [];

                foreach (var protocol in item.Value)
                {
#pragma warning disable SYSLIB1045
                    Match match = Regex.Match(protocol.Name, AppConstants.simple_number_pattern);
#pragma warning restore SYSLIB1045
                    if (match.Success)
                    {
                        type_numbers.Add(Convert.ToInt32(match.Groups["number"].Value));
                    }
                }
                type_numbers.Sort();

                Numbers.Add(item.Key, type_numbers);  
            }
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

