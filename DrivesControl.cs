using System.Xml.Linq;
using Logging;


namespace DrivesControl
{
    enum SettingsStatus
    {
        UNKNOWN,
        DRIVE_CONFIG_ERROR,
        DIRECTORY_ERROR,
        DIRECTORY_INSTALLED
    }


    readonly struct DrivesConfiguration
    {
        public static readonly List<string> drive_type = ["SOURCE", "DESTINATION"];
    }


    readonly struct Drive(string type, string dir)
    {
        public string Name { get; } = type;
        public readonly string Directory { get; } = dir;       
        public readonly bool Directory_Exist 
        { 
            get
            {   // проверка существования директории в системе
                if (System.IO.Directory.Exists(Directory))
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
        public List<Drive> Drives { get; private set; } = [];
        public bool Drives_Ready { get; private set; }
                         
        public XMLConfig()
        {
            // получаем конфигурацию
            XElement? drives_config = ConfigFiles.GetDrivesConfig();

            if (drives_config is not null)
            {
                foreach (string drive in DrivesConfiguration.drive_type)
                {
                    SettingsStatus config_status = SettingsStatus.UNKNOWN;

                    do
                    {




                    } while (config_status == SettingsStatus.DIRECTORY_ERROR);


                }





                // получаем путь из диска
                string? directory = drives_config.Element(drive_name)?.Value;      // проверить на исключение при повреждении имен дисков (тэга)

                if (directory is not null)
                {
                    Drive drive = new(drive_name, directory);
                    // создание диска и проверка существования директории в системе
                    if (drive.Directory_Exist)
                    {
                        Drives.Add(drive);
                        return SettingsStatus.DIRECTORY_INSTALLED;
                    }
                    else
                    {
                        return SettingsStatus.DIRECTORY_ERROR;
                    }
                }
                else
                {
                    return SettingsStatus.DRIVE_CONFIG_ERROR;
                }
            }
            else
            {
                return (SettingsStatus)ConfigFiles.ErrorCode.XML_CONFIG_FILE_ERROR;
            }


        }


        private SettingsStatus InstallDrive(string drive_name)        
        {
            // получаем конфигурацию
            XElement? drives_config = ConfigFiles.GetDrivesConfig();
            
            if (drives_config is not null)
            {
                // получаем путь из диска
                string? directory = drives_config.Element(drive_name)?.Value;      // проверить на исключение при повреждении имен дисков (тэга)
                        
                if (directory is not null)
                {

                    // method
                    Drive drive = new(drive_name, directory);   // проверить отдельно директорию
                    // создание диска и проверка существования директории в системе
                    if (drive.Directory_Exist)
                    {
                        Drives.Add(drive);
                        return SettingsStatus.DIRECTORY_INSTALLED;
                    }
                    else
                    {
                        return SettingsStatus.DIRECTORY_ERROR;
                    }
                }
                else
                {
                    return SettingsStatus.DRIVE_CONFIG_ERROR;                      
                }
            }
            else
            {
                return (SettingsStatus)ConfigFiles.ErrorCode.XML_CONFIG_FILE_ERROR;
            }
        }

        




        private bool PrepareToBackup()
        {
            SettingsStatus config_status = SettingsStatus.UNKNOWN;

            do
            {
                foreach (string drive in IDrivesConfiguration.drive_type)
                {
                    config_status = InstallDrive(drive);

                    switch (config_status)
                    {
                        case SettingsStatus.DIRECTORY_INSTALLED:
                            Console.WriteLine($"\nДиректория {drive} успешно установлена !");
                            break;

                        case SettingsStatus.DIRECTORY_ERROR:
                            Console.WriteLine($"\n{drive} - директория не существует ! Установите правильную >>");

                            string new_path = Console.ReadLine();
                            ConfigFiles.SetupDriveDirectory(drive, new_path); // null !!
                            break;

                        case SettingsStatus.DRIVE_CONFIG_ERROR:
                            Console.WriteLine($"\n{drive} - ошибка конфигурации ! Завершение работы >>");
                            return false;

                        case (SettingsStatus)ConfigFiles.ErrorCode.XML_CONFIG_FILE_ERROR:
                            Console.WriteLine("\nФайл настроек поврежден ! Завершение работы >>");
                            return false;

                        default:
                            Console.WriteLine("\nНеизвестная критическая ошибка !");
                            return false;
                    }
                }

            } while (config_status == SettingsStatus.DIRECTORY_ERROR);
            
            return true;
        }

        bool IDrivesConfiguration.PrepareToBackup()
        {
            return PrepareToBackup();
        }
    }


         
        

        
}
