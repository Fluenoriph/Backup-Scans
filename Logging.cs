using System.Xml.Linq;


namespace Logging
{
    struct ConfigFile
    {
        public string Xml_file { get; } = "config.xml";

        public ConfigFile () { }
    }


    class MaxNumbersPerMonth(int month_value)
    {
        private readonly List<string> items = ["F", "FA", "R", "RA", "M", "MA"];
        private static ConfigFile ConfigFile { get; }
        public List<int>? Values { get; private set; }

        private XmlAccess get_config = (x) =>
        {
            XDocument xdoc = XDocument.Load(ConfigFile.Xml_file);

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


    class RegistryKey(string reg_key)
    {
        public void Read()
        {
            XDocument xdoc = XDocument.Load(reg_key);

            var max_numbers = xdoc.Element("configuration")?
                .Element("internal_settings")
                .FirstAttribute(p => p.Attribute("month")?.Value == x);

            return max_numbers;
        }


    }

}

