using System.Text.RegularExpressions;


namespace BackupBlock
{
    struct MonthValues
    {
        public Dictionary<string, string> Table { get; } = table;

        private static readonly Dictionary<string, string> table = new()
        {
            ["Январь"] = "01",
            ["Февраль"] = "02",
            ["Март"] = "03",
            ["Апрель"] = "04",
            ["Май"] = "05",
            ["Июнь"] = "06",
            ["Июль"] = "07",
            ["Август"] = "08",
            ["Сентябрь"] = "09",
            ["Октябрь"] = "10",
            ["Ноябрь"] = "11",
            ["Декабрь"] = "12"
        };

        public MonthValues() { }
    }


    class BackupItem(string rgx_pattern)
    {
        private static DateTime current_date = DateTime.Now;
        private static string Year = current_date.Year.ToString();
        private static MonthValues monthes = new();
        public string Item_type { get; set; } = "*.pdf";
        public int Items_count { get; set; }

        public List<FileInfo> GetBackupingItems(string month, string path)
        {
            string date_file_pattern = string.Concat("\\d{2}\\.", monthes.Table[month], "\\.", Year, "\\.", Item_type, "$"); // lambda param. ??
            string full_pattern = string.Concat(rgx_pattern, date_file_pattern);
            Regex rgx = new(full_pattern, RegexOptions.IgnoreCase);

            DirectoryInfo dir = new(path);
            FileInfo[] file_list = dir.GetFiles(Item_type);       // null compatible vars (?) ???

            IEnumerable<FileInfo> backup_block = from file in file_list
                                                 where rgx.IsMatch(file.Name)
                                                 select file;

            List<FileInfo> result = [.. backup_block];
            Items_count = result.Count;

            return result;     // status ?  or null files !!!
        }
    }


    class ProtocolTypes(List<FileInfo> files)
    {
        private static List<string> protocol_types = ["ф", "фа", "р", "ра", "м", "ма"];   // ключи которые без "а" считать в Уссурийск
        private static string number_capture = "^(?<number>\\d+)-";
        public Dictionary<string, int> Type_Sums { get; } = [];
        public static List<string> Missing_Protocols { get; } = []; 

        private TypePattern rgx_number = static (type) => new($"{number_capture}{type}-", RegexOptions.IgnoreCase);

        private MissingProtocolNumbers none_numbers = static (names) =>
        {
            List<int> numbers = [];
            foreach (string s in names)
            {
                Match match = Regex.Match(s, number_capture);
                if (match.Success) { numbers.Add(Convert.ToInt32(match.Groups["number"].Value)); }
            }

            List<int> numbers_range = [];
            for (int i = numbers.Min(); i <= numbers.Max(); i++) { numbers_range.Add(i); }
                        
            return [.. numbers_range.Except(numbers)];
        };
           
        public void CalcProtocolTypes()
        {
            foreach (string type_i in protocol_types)
            {
                IEnumerable<string> type_block = from file in files
                                                 where rgx_number(type_i).IsMatch(file.Name)
                                                 select file.Name;

                List<string> types = [.. type_block];
                Type_Sums.Add(type_i, types.Count);

                if (types.Count > 2) { foreach (int x in none_numbers(types)) { Missing_Protocols.Add($"{x}-{type_i}"); } }          
            }
        }

        delegate Regex TypePattern(string protocol_type);
        delegate List<int> MissingProtocolNumbers(List<string> protocol_names);
    }


    namespace Logging
    {
        interface ISettings
        {
            int Value { get; set; }  // list ???? to types !!
            string Config_File { get; set; }

            void Read();
            void Write();


            // xml path file свойство
            // max value of protocol type
            // methods read write
        }



        // xml config 'month max number'




    }
}
