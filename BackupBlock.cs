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

        
    struct RgxPattern(string name_pattern)
    {
        private static readonly DateTime current_date = DateTime.Now;
        public string Item_type { get; } = "*.pdf";

        public readonly Regex Full_Pattern()
        {
            string date_file_pattern = string.Concat("\\d{2}\\.", $"0{MonthValues.Table[CurrentMonth.value]}", "\\.", current_date.Year.ToString(), "\\.", Item_type, "$");
            string full_pattern = string.Concat(name_pattern, date_file_pattern);

            return new(full_pattern, RegexOptions.IgnoreCase);
        }
    }


    enum FileBlockStatus
    {
        CURRENT_TYPE_NOT_EXIST,
        NONE_FILES_IN_CURRENT_PERIOD
    }


    class BackupItem(RgxPattern rgx_pattern, string path)
    {
        public int Items_count { get; set; } = 0;
        public List<FileInfo> Result_Block { get; set; } = [];
        public FileBlockStatus Status { get; set; }  // null or ok ???

        private FileInfo[]? GetTypeFiles()
        {
            DirectoryInfo dir = new(path);
            return dir.GetFiles(rgx_pattern.Item_type);
        }

        public void GetBackupingItems()
        {
            FileInfo[]? file_list = GetTypeFiles();

            if (file_list is not null)
            {
                IEnumerable<FileInfo>? backup_block = from file in file_list
                                                     where rgx_pattern.Full_Pattern().IsMatch(file.Name)
                                                     select file;

                if (backup_block is not null)
                {
                    Result_Block = [.. backup_block];
                    Items_count = Result_Block.Count;
                }
                else { Status = FileBlockStatus.NONE_FILES_IN_CURRENT_PERIOD; }
                 
            }
            else { Status = FileBlockStatus.CURRENT_TYPE_NOT_EXIST; }
        }
    }


    class ProtocolTypes(List<FileInfo> files)
    {
        private static List<string> protocol_types = ["ф", "фа", "р", "ра", "м", "ма"];   // ключи которые без "а" считать в Уссурийск
        private static string number_capture = "^(?<number>\\d+)-";
        private TypePattern rgx_number = static (type) => new($"{number_capture}{type}-", RegexOptions.IgnoreCase);

        public Dictionary<string, int> Type_Sums { get; } = [];

        public void Calculate()
        {
            foreach (string i in protocol_types)
            {
                IEnumerable<string>? type_block = from file in files
                                                 where rgx_number(i).IsMatch(file.Name)
                                                 select file.Name;

                if (type_block is not null)
                {
                    List<string> types = [.. type_block];
                    Type_Sums.Add(i, types.Count);


                }
                else { continue; }
                





                //if (types.Count > 2) { foreach (int x in none_numbers(types)) { Missing_Protocols.Add($"{x}-{i}"); } }          
            }
        }

        delegate Regex TypePattern(string protocol_type);
    }


    class MissingNumbers
    {
        public static List<string>? Missing_Protocols { get; }

        public Numbers get_type_numbers = static (x, y) =>
        {
            List<int> numbers = [];
            foreach (string s in x)
            {
                Match match = Regex.Match(s, y);
                if (match.Success) { numbers.Add(Convert.ToInt32(match.Groups["number"].Value)); }
            }
            return numbers;
        };

        /*public List<string> Calculate(List<string> protocol_type_names, string rgx_capture_pattern) // lambda
        {
            

            int max_number = numbers.Max();



            List<int> numbers_range = [];


            for (int i = numbers.Min(); i <= max_number; i++) { numbers_range.Add(i); }  // 'min' read xml max values




            return [.. numbers_range.Except(numbers)];
        }*/

        public delegate List<int> Numbers(List<string> protocol_type_names, string rgx_capture_pattern);
    }
}
