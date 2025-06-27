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


    interface IDrivesConfiguration
    {
        static List<string> drive_type = ["SOURCE", "DESTINATION"];

        SettingsStatus InstallDrive(string drive_name);
        bool PrepareToBackup();
    }


    struct Drive(string type)
    {
        private string? directory;
        public string Name { get; } = type;           
        public bool Directory_Ready { get; private set; }
        public string Directory
        {
            readonly get => directory ?? "NULL_DIRECTORY";  // " "
            // проверка директории на нулл или вообще существования
            set
            {
                if (System.IO.Directory.Exists(value))
                {
                    directory = value;
                    Directory_Ready = true;
                }
                else 
                { 
                    Directory_Ready = false; 
                }
            }
        }
    }


    class XMLConfig : IDrivesConfiguration
    {
        public List<Drive> Drives { get; private set; } = [];
        public bool Drives_Ready
        {
            get => PrepareToBackup();
        }
                    
        private SettingsStatus InstallDrive(string drive_name)        
        {
            // получаем конфигурацию
            XElement? drives_config = ConfigFiles.GetDrivesConfig();
            
            if (drives_config != null)
            {
                // получаем путь из диска
                string? directory = drives_config.Element(drive_name)?.Value;      // проверить на исключение при повреждении имен дисков (тэга)
                        
                if (directory != null)
                {
                    Drive drive = new(drive_name)
                    {
                        Directory = directory
                    };
                    // создание диска и проверка существования директории в системе
                    if (drive.Directory_Ready == false)
                    {
                        return SettingsStatus.DIRECTORY_ERROR;
                    }
                    else
                    {
                        Drives.Add(drive);
                        return SettingsStatus.DIRECTORY_INSTALLED;
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

        SettingsStatus IDrivesConfiguration.InstallDrive(string drive_name)
        {
            return InstallDrive(drive_name);
        }

        private bool PrepareToBackup()
        {
            SettingsStatus config_status = SettingsStatus.UNKNOWN;

            do
            {
                foreach (string drive in IDrivesConfiguration.drive_type)
                {
                    config_status = InstallDrive(drive);

                    if (config_status == (SettingsStatus)ConfigFiles.ErrorCode.XML_CONFIG_FILE_ERROR)
                    {
                        Console.WriteLine("\nФайл настроек поврежден ! Завершение работы >>");
                        return false;
                    }
                    else if (config_status == SettingsStatus.DRIVE_CONFIG_ERROR)
                    {
                        Console.WriteLine($"\n{drive} - ошибка конфигурации ! Завершение работы >>");
                        return false;
                    }
                    else if (config_status == SettingsStatus.DIRECTORY_ERROR)
                    {
                        Console.WriteLine($"\n{drive} - директория не существует ! Установите правильную >>");
                        
                        string new_path = Console.ReadLine();
                        ConfigFiles.SetupDriveDirectory(drive, new_path); // null !!
                    }
                    else
                    {
                        //Console.WriteLine($"\n{drive} OK !!!");
                        continue;
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
