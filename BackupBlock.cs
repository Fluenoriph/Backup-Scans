using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace BackupBlock
{
    readonly struct MonthValues
    {
        public Dictionary<string, int> Table { get; }

        private static readonly Dictionary<string, int> table = new()
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

        public MonthValues() { Table = table; }
    }


    class BackupItem(string path, string rgx_pattern)
    {
        private static DateTime current_date = DateTime.Now;   // static if to year ??
        private static string Year = current_date.Year.ToString();
        private static MonthValues monthes = new();
        public string Item_type { get; set; } = "*.pdf";
        public int Items_count { get; set; }

        public List<FileInfo> GetBackupingItems(string month)
        {
            string date_file_pattern = string.Concat("\\d{2}\\.", $"0{monthes.Table[month]}", "\\.", Year, "\\.", Item_type, "$"); 
            string full_pattern = string.Concat(rgx_pattern, date_file_pattern);
            Regex rgx = new(full_pattern, RegexOptions.IgnoreCase);

            DirectoryInfo dir = new(path);
            FileInfo[] file_list = dir.GetFiles(Item_type);       // null compatible vars (?) ???

            IEnumerable<FileInfo> backup_block = from file in file_list
                                                 where rgx.IsMatch(file.Name)
                                                 select file;

            List<FileInfo> result_block = [.. backup_block];
            Items_count = result_block.Count;

            return result_block;     // status ?  or null files !!!
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
        public static List<string> Missing_Protocols { get; } = [];

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

        public List<string> Calculate(List<string> protocol_type_names, string rgx_capture_pattern) // lambda
        {
            

            int max_number = numbers.Max();



            List<int> numbers_range = [];


            for (int i = numbers.Min(); i <= max_number; i++) { numbers_range.Add(i); }  // 'min' read xml max values




            return [.. numbers_range.Except(numbers)];
        }

        public delegate List<int> Numbers(List<string> protocol_type_names, string rgx_capture_pattern);
    }


    namespace Logging
    {
        interface ISettings
        {      
            void Read(); 
            void Write(List<int> max_numbers);
        }


        class XmlConfig(int month_value) : ISettings
        {
            private readonly List<string> items = ["F", "FA", "R", "RA", "M", "MA"];
            private static string Config_File { get; } = "config.xml";
            public List<int> Values { get; private set; } = [];

            private XmlAccess get_config = (x) =>
            {
                XDocument xdoc = XDocument.Load(Config_File);

                var max_numbers = xdoc.Element("configuration")?
                    .Elements("max_numbers")
                    .FirstOrDefault(p => p.Attribute("month")?.Value == x);

                return max_numbers;
            };

            void ISettings.Read()
            {
                if (month_value is not 1)
                {
                    var xvalue = get_config($"{month_value + 1}");      
                    foreach (var item in items) { Values.Add(Convert.ToInt32(xvalue?.Attribute(item)?.Value)); }                    
                }
                else { return; }
            }
            
            void ISettings.Write(List<int> max_numbers)
            {
                var xvalue = get_config(month_value.ToString());

                for (int i = 0; i <items.Count; i++)
                {
                    int x = max_numbers[i];

                    if (x != 0) 
                    {
                        var y = xvalue?.Element(items[i]);
                        y.Value = x.ToString(); // if == null....
                    }
                    else { continue; }
                }
            }

            delegate XElement? XmlAccess(string month_value);
        }

        
    }
}
