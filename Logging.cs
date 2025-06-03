using System.Xml.Linq;


namespace Logging
{
    class ConfigFile
    {
        private readonly string xml_settings_file = "C:\\Users\\Asus machine\\source\\repos\\Backup Scans\\config.xml";   // относительный ....

        public XDocument GetDoc() { return XDocument.Load(xml_settings_file); }
    }


    class MaxNumbersPerMonth(int month_value)
    {
        private readonly List<string> items = ["F", "FA", "R", "RA", "M", "MA"];
        private static ConfigFile Config_File { get; }
        public List<int>? Values { get; private set; }

        private XmlAccess get_config = (x) =>
        {
            XDocument xdoc = Config_File.GetDoc();

            var max_numbers = xdoc.Element("configuration")?
                .Elements("max_numbers")
                .FirstOrDefault(p => p.Attribute("month")?.Value == x);

            return max_numbers;
        };

        void Read()
        {
            if (month_value is not 1)
            {
                var xvalue = get_config($"{month_value + 1}");
                foreach (var item in items) { Values.Add(Convert.ToInt32(xvalue?.Attribute(item)?.Value)); }
            }
            else { return; }
        }

        void Write(List<int> max_numbers)
        {
            var xvalue = get_config(month_value.ToString());

            for (int i = 0; i < items.Count; i++)
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


    class RegKeyInXML
    {
        private static ConfigFile Config_File = new();

        public static string? GetPath()
        {
            XDocument xdoc = Config_File.GetDoc();
            XElement? config = xdoc.Element("configuration");
            XElement? x = config?.Element("internal_settings");
            string? key = x?.Element("reg_key_path")?.Value;
            Console.WriteLine(key);
            
            if ((config != null) & (x != null) & (key != null)) { return key; }
            else { return null; } 
        }

        public static void SetPath ()
        {
            ///////
        }
    }

}

