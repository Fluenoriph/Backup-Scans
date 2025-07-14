using System.Xml.Linq;


namespace Logging
{
    class DrivesConfiguration
    {
        private const string drives_config_file = "C:\\Users\\Asus machine\\source\\repos\\Backup Scans\\drives_config.xml";   // относительный ....
                
        public static XElement? Config_Element 
        { 
            get
            {
                XDocument doc = XDocument.Load(drives_config_file);
                return doc.Element("configuration");                 // если повреждение тэга, то исключение, если просто другое имя то 'null' -- exit
            }
        }
                
        public static void SetupDriveDirectory(string drive_name, string path)
        {
            XElement? dir = Config_Element?.Element(drive_name);

            if (dir is not null)
            {
                dir.Value = path;
                Config_Element?.Save(drives_config_file);

                Console.WriteLine($"\n{drive_name} is installed !!");     // out OK !!
            } 
        }
    }

    


    

}

