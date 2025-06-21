using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace BackupBlock
{
    readonly struct MonthValues
    {
        public static Dictionary<string, int> Table { get; } = new Dictionary<string, int>
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
        public Dictionary<string, int> Table { get; set; } = new Dictionary<string, int>
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

    struct RgxPattern(string current_month, string name_pattern)   
    {
        private static readonly DateTime current_date = DateTime.Now;
        public string File_Type { get; } = "*.pdf";

        public readonly Regex Full_Pattern()
        {
            string date_pattern = string.Concat("\\d{2}\\.", $"0{MonthValues.Table[current_month]}", "\\.", current_date.Year.ToString(), "\\.", File_Type, "$");
            string full_pattern = string.Concat(name_pattern, date_pattern);

            return new(full_pattern, RegexOptions.IgnoreCase);
        }
    }


    enum FileBlockStatus
    {
        FILES_DO_NOT_EXIST,
        NONE_FILES_IN_CURRENT_PERIOD,
        FILES_FOUND
    }


    class BackupItem(RgxPattern rgx_pattern, string drive_directory)
    {
        public List<FileInfo> Result_Files { get; set; } = [];
                        
        private FileInfo[]? GetAllFilesToType()
        {
            DirectoryInfo directory = new(drive_directory);
            return directory.GetFiles(rgx_pattern.File_Type);
        }

        public FileBlockStatus GetBackupingItems()
        {
            FileInfo[]? files = GetAllFilesToType();

            if (files != null)
            {
                IEnumerable<FileInfo>? backup_block = from file in files
                                                      where rgx_pattern.Full_Pattern().IsMatch(file.Name)
                                                      select file;

                if (backup_block != null)
                {
                    Result_Files = [.. backup_block];
                    
                    return FileBlockStatus.FILES_FOUND;
                }
                else 
                { 
                    return FileBlockStatus.NONE_FILES_IN_CURRENT_PERIOD; 
                }
            }
            else 
            { 
                return FileBlockStatus.FILES_DO_NOT_EXIST; 
            }
        }
    }


    class Protocols(List<FileInfo> backup_files)
    {
        private static readonly List<string> protocol_types = ["ф", "фа", "р", "ра", "м", "ма"];
        
        private const string number_capture = "^(?<number>\\d+)-";
        private readonly TypePattern rgx_number_type = (type) => new($"{number_capture}{type}-", RegexOptions.IgnoreCase);
        
        public List<int> Type_Sums { get; set; } = [];
        public List<string> Missing_Protocols { get; } = [];

        public void Calc() // directory ????   func name ??
        {
            for (int type_index = 0; type_index < protocol_types.Count; type_index++)
            {
                IEnumerable<string>? block_of_type = from file in backup_files
                                                  where rgx_number_type(protocol_types[type_index]).IsMatch(file.Name)
                                                  select file.Name;
                                
                if (block_of_type != null)
                {
                    List<string> type_list = [.. block_of_type];


                    Type_Sums.Add(type_list.Count);


                    if (type_list.Count > 2)
                    {
                        List<int> numbers = GetNumbersAtType(type_list);





                        /*foreach (int number in numbers)
                        {
                            Console.WriteLine(number);
                        }*/
                    }

                }
                else
                {
                    Type_Sums.Add(0);
                }
                

                
                         
            }
        }

        private static List<int> GetNumbersAtType(List<string> protocol_types)
        {
            List<int> numbers = [];

            foreach (string protocol in protocol_types)
            {
                Match match = Regex.Match(protocol, number_capture);

                if (match.Success) 
                { 
                    numbers.Add(Convert.ToInt32(match.Groups["number"].Value)); 
                }
                // else ???
            }
            
            return numbers;
        }

            
        private List<int> CreateRange(int start, int end)
        {

        }
        






        private void CalcMissingNumbers(List<int> numbers)
        {


        }

        

        delegate Regex TypePattern(string protocol_type);
    }


    
}
