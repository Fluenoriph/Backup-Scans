using System.Xml.Linq;


namespace Logging
{
    class ConfigFiles
    {
        private const string drives_config_file = "C:\\Users\\Asus machine\\source\\repos\\Backup Scans\\drives_config.xml";   // относительный ....
        private const string max_numbers_file = "C:\\Users\\Asus machine\\source\\repos\\Backup Scans\\max_numbers.xml";
        internal enum ErrorCode
        {
            XML_CONFIG_FILE_ERROR,
            VALUE_IS_JANUARY__NO_UPPER_NUMBERS
        }

        public static XElement? GetDrivesConfig()
        {
            XDocument doc = XDocument.Load(drives_config_file);
            return doc.Element("configuration");                    // если повреждение тэга, то исключение, если просто другое имя то 'null'
        }

        public static void SetupDriveDirectory(string drive_name, string path)
        {
            XElement? xdoc = GetDrivesConfig();

            if (xdoc != null)
            {
                XElement? dir = xdoc.Element(drive_name);

                if (dir != null)
                {
                    dir.Value = path;
                    xdoc.Save(drives_config_file);
                    Console.WriteLine($"\n{drive_name} is installed !!");     // out OK !!
                }
            }  // else ???
        }

        
    }


    class MaxNumbersPerMonth(int month_value)
    {
        private readonly List<string> items = ["F", "FA", "R", "RA", "M", "MA"];
        public ConfigFiles.ErrorCode ErrorStatus { get; set; }
        public List<int> Values { get; private set; } = [];

        private static XElement? GetNumbersTree()
        {
            XDocument xdoc = null;

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
                else { ErrorStatus = ConfigFiles.ErrorCode.XML_CONFIG_FILE_ERROR; }   // xml error
            }
            else { ErrorStatus = ConfigFiles.ErrorCode.VALUE_IS_JANUARY__NO_UPPER_NUMBERS; }
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


    

}

