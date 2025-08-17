using System.Xml.Linq;
using TextData;
using Logging;


namespace DrivesControl
{
    enum SettingsStatus   // is's need, if write to log_error_file
    {
        UNKNOWN,
        DRIVE_CONFIG_ERROR,
        DIRECTORY_ERROR,
        DIRECTORY_INSTALLED
    }

    
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
        public List<Drive> Drives { get; } = new(2);
        
        public XMLConfig()
        {
            // получаем конфигурацию
            XElement? drives_config = DrivesConfiguration.Config_Element;
            
            if (drives_config is not null)
            {
                foreach (string drive_name in AppConstants.drive_type)
                {
                    // получаем путь из диска
                    var directory = drives_config.Element(drive_name)?.Value;      // проверить на исключение при повреждении имен дисков (тэга) -- exit

                    if (directory is not null)
                    {
                        Drives.Add(SetupDrive(drive_name, directory));
                    }
                    else
                    {
                        Console.WriteLine($"\n{drive_name}: DRIVE_CONFIG_ERROR");
                        System.Environment.Exit(0);
                        //SettingsStatus.DRIVE_CONFIG_ERROR;
                    }
                }

                Console.WriteLine("\nВсе готово к копированию !");
            }
            else
            {
                Console.WriteLine("\nErrorCode.XML_CONFIG_FILE_ERROR");
                System.Environment.Exit(0);
                //(SettingsStatus)ConfigFiles.ErrorCode.XML_CONFIG_FILE_ERROR;
            }
        }

        // установка директорий в диски
        private static Drive SetupDrive(string drive_name, string directory)
        {
            string setup_directory = directory;
            Drive drive;
            bool dir_status;

            // проверка существования директории в системе
            do
            {
                drive = new(drive_name, setup_directory);

                dir_status = drive.Directory_Exist;

                if (dir_status)
                {
                    Console.WriteLine($"\nДиректория {drive_name} успешно установлена !");
                    break;
                }
                else
                {
                    Console.WriteLine($"\n{drive_name} - директория не существует ! Установите правильную >>");
                    string new_path = Console.ReadLine();

                    setup_directory = new_path;

                    Logging.DrivesConfiguration.SetupDriveDirectory(drive_name, new_path); // null !!
                }

            } while (dir_status == false);

            return drive;
        }                
    }
}