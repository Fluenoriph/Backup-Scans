using System.Xml.Linq;


namespace Logging
{
    class ConfigFile
    {
        private static string xml_settings_file = "C:\\Users\\Asus machine\\source\\repos\\Backup Scans\\config.xml";   // относительный ....

        public static XDocument GetDoc() { return XDocument.Load(xml_settings_file); }
    }


    class MaxNumbersPerMonth(int month_value)
    {
        private readonly List<string> items = ["F", "FA", "R", "RA", "M", "MA"];
        public List<int> Values { get; private set; } = [];

        private static XElement? GetNumbersTree()
        {
            XDocument xdoc = ConfigFile.GetDoc();

            XElement? config = xdoc.Element("configuration");
            XElement? numbers = config?.Element("max_numbers");

            if ((config is not null) & (numbers is not null)) 
            {
                Console.WriteLine("Tree OK");
                return numbers; 
            }
            else 
            {
                Console.WriteLine("Tree Bad");
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
                else { Console.WriteLine("\nxml null\n"); }   // error type

            }
            else 
            {
                Console.WriteLine("\nValue is January\n");
                return; 
            }
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
        //private static ConfigFile Config_File = new();

        public static string? GetPath()
        {
            XDocument xdoc = ConfigFile.GetDoc();
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

