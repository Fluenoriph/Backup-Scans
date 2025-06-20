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

        
    struct RgxPattern(string name_pattern)   // parameter ????
    {
        private static readonly DateTime current_date = DateTime.Now;
        public string Item_type { get; } = "*.pdf";

        public readonly Regex Full_Pattern()
        {
            string date_file_pattern = string.Concat("\\d{2}\\.", $"0{MonthValues.Table[CurrentMonth.Value]}", "\\.", current_date.Year.ToString(), "\\.", Item_type, "$");
            string full_pattern = string.Concat(name_pattern, date_file_pattern);

            return new(full_pattern, RegexOptions.IgnoreCase);
        }
    }


    enum FileBlockStatus
    {
        FILES_DO_NOT_EXIST,
        NONE_FILES_IN_CURRENT_PERIOD,
        FILES_FOUND
    }


    class BackupItem(RgxPattern rgx_pattern, string path)
    {
        public List<FileInfo> Result_Block { get; set; } = [];
        public int Items_count { get; set; }
                
        private FileInfo[]? GetAllFilesToType()
        {
            DirectoryInfo dir = new(path);
            return dir.GetFiles(rgx_pattern.Item_type);
        }

        public FileBlockStatus GetBackupingItems()
        {
            FileInfo[]? file_list = GetAllFilesToType();

            if (file_list != null)
            {
                IEnumerable<FileInfo>? backup_block = from file in file_list
                                                      where rgx_pattern.Full_Pattern().IsMatch(file.Name)
                                                      select file;

                if (backup_block != null)
                {
                    Result_Block = [.. backup_block];
                    Items_count = Result_Block.Count;

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




    class ProtocolTypes(List<FileInfo> files)
    {
        private static readonly List<string> protocol_types = ["ф", "фа", "р", "ра", "м", "ма"];   
        private const string number_capture = "^(?<number>\\d+)-";
        private readonly TypePattern rgx_number_type = (type) => new($"{number_capture}{type}-", RegexOptions.IgnoreCase);
        
        public List<int> Type_Sums { get; set; } = [];
        public List<string> Missing_Protocols { get; } = [];

        public void Calc() // directory ????   func name ??
        {
            for (int i = 0; i < protocol_types.Count; i++)
            {
                IEnumerable<string>? type_block = from file in files
                                                  where rgx_number_type(protocol_types[i]).IsMatch(file.Name)
                                                  select file.Name;
                                
                if (type_block != null)
                {
                    List<string> types = [.. type_block];
                    Type_Sums.Add(types.Count);

                    if (types.Count > 2)
                    {
                        List<int> numbers = GetNumbersAtType(types);





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
