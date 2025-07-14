using System.Xml.Linq;


namespace Logging
{
    class DrivesConfiguration
    {
        private const string drives_config_file = "C:\\Users\\Mahabhara\\source\\repos\\Fluenoriph\\Backup-Scans\\drives_config.xml";   // относительный ....

        static DrivesConfiguration()
        {
            XDocument doc = XDocument.Load(drives_config_file);
            Config_Element = doc.Element("configuration");                 // если повреждение тэга, то исключение, если просто другое имя то 'null' -- exit
        }

        public static XElement? Config_Element { get; set; }
        
        public static void SetupDriveDirectory(string drive_name, string path)
        {
            XElement? dir = Config_Element?.Element(drive_name);

            if (dir is not null)
            {
                dir.Value = path;
                Config_Element?.Save(drives_config_file);

                Console.WriteLine($"\n{drive_name} is installed !!");     // out OK !!
            } // else ??
        }
    }

    


    

}

