using System.Xml.Linq;
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


    readonly struct DrivesConfiguration
    {
        public static readonly List<string> drive_type = ["SOURCE", "DESTINATION"];
    }


    class Drive(string type)
    {
        public string Name { get; } = type;
        public string Directory { get; set; } = "";
        public bool Directory_Exist
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
        public List<Drive> Drives { get; } = [new(DrivesConfiguration.drive_type[0]), new(DrivesConfiguration.drive_type[1])];
        public bool Drives_Ready { get; private set; }

        public XMLConfig()
        {
            // получаем конфигурацию
            XElement? drives_config = ConfigFiles.GetDrivesConfig();
            List<string> dirs = [];

            if (drives_config is not null)
            {
                foreach (string drive_name in DrivesConfiguration.drive_type)
                {
                    // получаем путь из диска
                    string? directory = drives_config.Element(drive_name)?.Value;      // проверить на исключение при повреждении имен дисков (тэга) -- exit

                    if (directory is not null)
                    {
                        dirs.Add(directory);
                    }
                    else
                    {
                        Console.WriteLine($"\n{drive_name}: DRIVE_CONFIG_ERROR");
                        System.Environment.Exit(0);
                        //SettingsStatus.DRIVE_CONFIG_ERROR;
                    }
                }
            }
            else
            {
                Console.WriteLine("\nErrorCode.XML_CONFIG_FILE_ERROR");
                System.Environment.Exit(0);
                //(SettingsStatus)ConfigFiles.ErrorCode.XML_CONFIG_FILE_ERROR;
            }
            // установка директорий в диски
            for (int drive_index = 0; drive_index < Drives.Count; drive_index++)
            {
                bool dir_status;
                // проверка существования директории в системе
                do
                {
                    Drives[drive_index].Directory = dirs[drive_index];

                    dir_status = Drives[drive_index].Directory_Exist;

                    if (dir_status)
                    {
                        Console.WriteLine($"\nДиректория {DrivesConfiguration.drive_type[drive_index]} успешно установлена !");
                    }
                    else
                    {
                        Console.WriteLine($"\n{DrivesConfiguration.drive_type[drive_index]} - директория не существует ! Установите правильную >>");
                        string new_path = Console.ReadLine();

                        dirs[drive_index] = new_path;
                        ConfigFiles.SetupDriveDirectory(DrivesConfiguration.drive_type[drive_index], new_path); // null !!
                    }

                } while (dir_status == false);
            }

            Console.WriteLine("\nВсе готово к копированию !");
        }
    }
}