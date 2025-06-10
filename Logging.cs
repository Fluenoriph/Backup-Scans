using System.Xml.Linq;


namespace Logging
{
    struct ConfigFile
    {
        private static string xml_settings_file = "C:\\Users\\Asus machine\\source\\repos\\Backup Scans\\config.xml";   // относительный ....
        public enum ErrorCode
        {
            XML_CONFIG_FILE_ERROR,
            VALUE_IS_JANUARY__NO_UPPER_NUMBERS
        }

        public static XDocument GetDocument() { return XDocument.Load(xml_settings_file); }
    }


    class MaxNumbersPerMonth(int month_value)
    {
        private readonly List<string> items = ["F", "FA", "R", "RA", "M", "MA"];
        public ConfigFile.ErrorCode ErrorStatus { get; set; }
        public List<int> Values { get; private set; } = [];

        private static XElement? GetNumbersTree()
        {
            XDocument xdoc = ConfigFile.GetDocument();

            XElement? config = xdoc.Element("configuration");     // bad tag directory - exception !
            XElement? numbers = config?.Element("max_numbers");

            if ((config is not null) & (numbers is not null)) 
            {
                Console.WriteLine("Tree OK");
                return numbers; 
            }
            else 
            {
                Console.WriteLine("Tree Bad");    // xml error
                return null; 
            }
        }

        private void GetMaxNumbers(XElement month)
        {
            for (int i = 0; i < items.Count; i++)
            {
                XElement? number = month.Element(items[i]);
                Values.Add(Convert.ToInt32(number?.Value));
            }
        }

        public void Read()
        {
            if (month_value is not 1)
            {
                XElement? xnumbers = GetNumbersTree();

                if (xnumbers is not null)
                {
                    foreach (XElement month in xnumbers.Elements("month"))
                    {
                        XAttribute? month_number = month.Attribute("value");

                        if (month_number?.Value == $"{month_value - 1}")
                        {
                            Console.WriteLine($"{month_number?.Value}");
                            GetMaxNumbers(month);
                        }
                    }
                }
                else { ErrorStatus = ConfigFile.ErrorCode.XML_CONFIG_FILE_ERROR; }   // xml error
            }
            else { ErrorStatus = ConfigFile.ErrorCode.VALUE_IS_JANUARY__NO_UPPER_NUMBERS; }
        }

        public void Write(List<int> max_numbers)
        {
            var xvalue = GetNumbersTree();

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
    }


    class RegKeyInXML
    {
        public static string? GetPath()
        {
            XDocument xdoc = ConfigFile.GetDocument();
            XElement? config = xdoc.Element("configuration");    // xml error
            XElement? target_tree = config?.Element("internal_settings");   // xml error
            string? key = target_tree?.Element("reg_key_path")?.Value;     //  xml error
            
            Console.WriteLine(key); ///////
            
            if ((config != null) & (target_tree != null) & (key != null)) { return key; }
            
            else { return null; } 
        }

        public static void SetPath ()
        {
            ///////
        }
    }

}

