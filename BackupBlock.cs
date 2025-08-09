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
               
        
    interface IProtocolNumbers
    {        
        private protected static List<int> ConvertToNumbers(List<FileInfo> protocols, string number_capture_pattern)
        {
            List<int> numbers = [];

            foreach (var protocol in protocols)
            {
                Match match = Regex.Match(protocol.Name, number_capture_pattern);

                if (match.Success)
                {
                    numbers.Add(Convert.ToInt32(match.Groups["number"].Value)); // test number !! print
                }

                // нужно ли ??
                else
                {
                    Console.WriteLine("\nОшибка захвата номера протокола !"); // shutdown app ??
                }
            }

            numbers.Sort();
            return numbers;
        }
    }    


    interface ISortedNames
    {
        // abs field number string ???
        private protected static List<string> CreateSortedNames(List<int> numbers, List<FileInfo> protocols)
        {
            List<string> names = [];

            foreach (int number in numbers)
            {
                foreach (FileInfo protocol in protocols)
                {
                    if (protocol.Name.StartsWith($"{number}")) // разное для еиас !!
                    {
                        names.Add(protocol.Name);
                        break;
                    }
                }
            }
            return names;
        }
    }

    
    class SimpleProtocolsNumbers : IProtocolNumbers
    {
        public Dictionary<string, List<int>> Numbers { get; } = [];

        public SimpleProtocolsNumbers(Dictionary<string, List<FileInfo>> files)
        {
            foreach (var item in files)
            {                
                Numbers.Add(item.Key, IProtocolNumbers.ConvertToNumbers(item.Value, AppConstants.simple_number_pattern));  
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

