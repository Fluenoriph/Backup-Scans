using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace BackupBlock
{
    readonly struct MonthValues
    {
        public Dictionary<string, int> Table { get; } = new Dictionary<string, int>
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


    struct RgxPattern(string name_pattern, string month)
    {
        private static readonly DateTime current_date = DateTime.Now;
        private static readonly MonthValues monthes = new();
        public string Item_type { get; } = "*.pdf";

        public readonly Regex Full_Pattern()
        {
            string date_file_pattern = string.Concat("\\d{2}\\.", $"0{monthes.Table[month]}", "\\.", current_date.Year.ToString(), "\\.", Item_type, "$");
            string full_pattern = string.Concat(name_pattern, date_file_pattern);

            return new(full_pattern, RegexOptions.IgnoreCase);
        }
    }


    class BackupItem(string path, RgxPattern rgx_pattern)
    {
        public int? Items_count { get; set; }
        public List<FileInfo>? Result_Block { get; set; } // no ??

        public void GetBackupingItems()
        {
            DirectoryInfo dir = new(path);
            FileInfo[]? file_list = dir.GetFiles(rgx_pattern.Item_type);

            if (file_list is not null)
            {
                IEnumerable<FileInfo> backup_block = from file in file_list
                                                     where rgx_pattern.Full_Pattern().IsMatch(file.Name)
                                                     select file;

                Result_Block = [.. backup_block];
                Items_count = Result_Block.Count;   // return list or null !
            }
            else { return; }
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
            foreach (string type_i in protocol_types)
            {
                IEnumerable<string> type_block = from file in files
                                                 where rgx_number(type_i).IsMatch(file.Name)
                                                 select file.Name;

                List<string> types = [.. type_block];
                Type_Sums.Add(type_i, types.Count);





                //if (types.Count > 2) { foreach (int x in none_numbers(types)) { Missing_Protocols.Add($"{x}-{type_i}"); } }          
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
