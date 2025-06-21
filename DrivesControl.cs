using System.Xml.Linq;
using Logging;


namespace DrivesControl
{
    enum SettingsStatus
    {
        UNKNOWN,
        DRIVE_CONFIG_ERROR,
        PATH_DOES_NOT_EXIST,
        PATH_INSTALLED,
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
        public SettingsStatus Directory_Status { get; private set; }
        public string Directory
        {
            readonly get => directory ?? "NULL_DIRECTORY";  // " "
            // проверка директории на нулл или вообще существования
            set
            {
                if (System.IO.Directory.Exists(value))
                {
                    directory = value;
                    Directory_Status = SettingsStatus.PATH_INSTALLED;
                }
                else 
                { 
                    Directory_Status = SettingsStatus.PATH_DOES_NOT_EXIST; 
                }
            }
        }
    }


    class XMLConfig : IDrivesConfiguration
    {
        public List<Drive> Drives { get; private set; } = [];               
                    
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
                    if (drive.Directory_Status == SettingsStatus.PATH_DOES_NOT_EXIST)
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

        public bool PrepareToBackup()
        {
            SettingsStatus config_status = SettingsStatus.UNKNOWN;

            do
            {
                foreach (string drive in IDrivesConfiguration.drive_type)
                {
                    config_status = InstallDrive(drive);

                    if (config_status == (SettingsStatus)ConfigFiles.ErrorCode.XML_CONFIG_FILE_ERROR)
                    {
                        Console.WriteLine("\nXML Error ! Exit >>");
                        return false;
                    }
                    else if (config_status == SettingsStatus.DRIVE_CONFIG_ERROR)
                    {
                        Console.WriteLine($"\n{drive} - Drive Config Error ! Exit >>");
                        return false;
                    }
                    else if (config_status == SettingsStatus.DIRECTORY_ERROR)
                    {
                        Console.WriteLine($"\n{drive}: Directory Error ! Setup New Dir ! >>>");
                        
                        string new_path = Console.ReadLine();
                        ConfigFiles.SetupDriveDirectory(drive, new_path); // null !!
                    }
                    else
                    {
                        Console.WriteLine($"\n{drive} OK !!!");
                        continue;
                    }
                }

            } while (config_status == SettingsStatus.DIRECTORY_ERROR);
            
            return true;
        }
    }


         
        

        
}
