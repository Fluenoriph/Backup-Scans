using System.Xml.Linq;
using TextData;
using Logging;


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


    class XMLConfig
    {
        private readonly DrivesConfiguration self_obj_drives_config = new();
        public List<Drive> Drives { get; } = new(2);
        
        public XMLConfig()
        {
            // получаем конфигурацию            
            if (self_obj_drives_config.Config_Sector is not null)       // проверить на нулл в низкоуровневом классе
            {
                foreach (string drive_name in AppConstants.drive_type)
                {
                    // получаем путь из тэга диска
                    var directory = self_obj_drives_config.Config_Sector.Element(drive_name)?.Value;      // проверить на исключение при повреждении имен дисков (тэга) -- exit

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
                                        
                    self_obj_drives_config.SetupDriveDirectory(drive_name, setup_directory);
                    AppInfoConsoleOut.ShowInstallDirectory(drive_name);
                }

            } while (directory_status == false);

            return self_obj_drive;
        }                
    }
}