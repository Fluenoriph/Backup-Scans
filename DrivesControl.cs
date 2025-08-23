using System.Xml.Linq;
using TextData;

// name rool disable
namespace DrivesControl
{
    class Drive(string type, string directory)
    {
        public string Name { get; } = type;
        public DirectoryInfo Directory { get; } = new(directory);
        public bool Directory_Exist
        {
            get
            {   
                if (Directory.Exists)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }





    class XLogFiles  // проверка
    {
        //public static XDocument x_doc_year = XDocument.Load(AppConstants.year_log_file);
        //public static XDocument x_doc_monthes = XDocument.Load(AppConstants.month_logs_file);
        public static XDocument x_doc_
    }




    abstract class DrivesConfigurationnew
    {
        private protected XDocument? X_doc_drives_config;




    }

    // по одной директории
    class DrivesConfiguration
    {
        private XDocument Xdoc { get; } = XDocument.Load(AppConstants.drives_config_file);
        private XElement? Config_Sector { get; }

        public List<Drive> Drives { get; } = new(2);
        
        public DrivesConfiguration()
        {
            Config_Sector = Xdoc.Element("configuration");
            // errors check !!!
            // если повреждение тэга, то исключение, если просто другое имя то 'null' -- exit

            // получаем конфигурацию            
            if (Config_Sector is not null)       
            {
                foreach (string drive_name in AppConstants.drive_type)
                {
                    // получаем путь из тэга диска
                    var directory = Config_Sector.Element(drive_name)?.Value;      // проверить на исключение при повреждении имен дисков (тэга) -- exit

                    if (directory is not null)
                    {
                        Drives.Add(SetupDrive(drive_name, directory));
                    }
                    else
                    {
                        Console.WriteLine($"\n{drive_name}: DRIVE_CONFIG_ERROR");
                        Environment.Exit(0);
                    }
                }                
            }
            else
            {
                Console.WriteLine("\nErrorCode.XML_CONFIG_FILE_ERROR");
                Environment.Exit(0);
            }
        }
        // установка директорий в диски
        private Drive SetupDrive(string drive_name, string directory)
        {
            string setup_directory = directory;
            Drive self_obj_drive;
            bool directory_status;
            // проверка существования директории в системе
            do
            {
                self_obj_drive = new(drive_name, setup_directory);
                directory_status = self_obj_drive.Directory_Exist;

                if (directory_status)
                {
                    AppInfoConsoleOut.ShowDirectorySetupTrue(drive_name, setup_directory);
                    break;
                }
                else
                {
                    AppInfoConsoleOut.ShowDirectoryExistFalse(drive_name);
                    
                    setup_directory = InputNoNullText.GetRealText();
                                        
                    WriteDriveDirectory(drive_name, setup_directory);
                    AppInfoConsoleOut.ShowInstallDirectory(drive_name);
                }

            } while (directory_status == false);

            return self_obj_drive;
        }

        private void WriteDriveDirectory(string drive_name, string path)
        {
            var directory = Config_Sector!.Element(drive_name);

            if (directory is not null)
            {
                directory.Value = path;
                Xdoc.Save(AppConstants.drives_config_file);
            }
            else
            {
                // xml error exit ??
            }
        }
    }
}
