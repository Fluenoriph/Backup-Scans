using DrivesControl;
using Microsoft.Win32;


const string SETTINGS_FULL_KEY = "HKEY_CURRENT_USER\\Software\\Ivan_Bogdanov\\Backup_Scans";

//const string SOURCE_DIR = "C:\\Users\\Asus machine\\Desktop\\Files\\сканы";
//const string DESTINATION_DIR = "C:\\Users\\Asus machine\\Desktop\\Files\\result_test";


SettingsInWinRegistry x = new(SETTINGS_FULL_KEY);
x.GetSettings();

foreach (var Drive in x.Drives)
{
    Console.WriteLine(Drive.Name);
}


namespace DrivesControl
{
    enum PathState
    {
        DOES_NOT_EXIST,             // не существует
        NOT_INSTALLED,              // не установлена
        DROPPED,                    // сброшена ??????
        INSTALLED,                  // УСТАНОВЛЕНА
        READY_TO_WORK,              // ГОТОВА К РАБОТЕ
    }


    interface IDriveSettings
    {
        static string[] drives = ["source", "destination"];
        
        List<string> GetSettings();
        void SetupDrive(string drive_type, string path);
    }
        

    struct WorkPath
    {
        private string name;        
        private PathState status;
        
        public PathState Status { get; set; }
        public string Name
        {
            set   // доступ ????
            {
                if (Directory.Exists(value))      // Проверить также диск !!!!
                {
                    name = value;
                    Status = PathState.READY_TO_WORK;
                    Console.WriteLine("Dir set OK !");
                }
                else
                {
                    Status = PathState.DOES_NOT_EXIST;
                    Console.WriteLine("Dir not exist !");
                }
            }
            get { return name; }
        }
    }


    class SettingsInWinRegistry(string key_path) : IDriveSettings
    {
        private string Key { get; set; } = key_path;
        
        public List<string> GetSettings()
        {
            List<string> dirs = [];

            for (int i = 0; i < IDriveSettings.drives.Length; i++)
            {
                string? dir_name = (string?)Registry.GetValue(Key, IDriveSettings.drives[i], "None");

                if (dir_name == null)
                {
                    Console.WriteLine("Reg Key-Path Not Found !!!\n");
                    Console.WriteLine($"Setup Reg Key >>> Set Drive {IDriveSettings.drives[i]} Data !");
                    string? data = Console.ReadLine();
                    SetupDrive(IDriveSettings.drives[i], data);
                }
                else if (dir_name == "None")
                {
                    Console.WriteLine($"None Key !!!\nSetup Drive {IDriveSettings.drives[i]} Value >>>");
                    string? data = Console.ReadLine();
                    SetupDrive(IDriveSettings.drives[i], data);
                }
                else
                {
                    dirs.Add(dir_name);
                }
            }
            return dirs;
        }

        public void SetupDrive(string drive_type, string path)
        {
            Registry.SetValue(Key, drive_type, path, RegistryValueKind.String);

        }               
    }
}


namespace BackupBlock
{
    abstract class BackupItem(string rgx_pattern)
    {
        DateOnly current_date;
        public string item_type = "*.pdf"; 
        public string Rgx_pattern {  get; set; } = rgx_pattern;         
        //time_span ???
        public abstract int Items_count { get; set; }

        public Dictionary<string, string> monthes_table = new()
        {
            ["Январь"] = "01",
            ["Февраль"] = "02",
            ["Март"] = "03",
            ["Апрель"] = "04",
            ["Май"] = "05",
            ["Июнь"] = "06",
            ["Июль"] = "07",
            ["Август"] = "08",
            ["Сентябрь"] = "09",
            ["Октябрь"] = "10",
            ["Ноябрь"] = "11",
            ["Декабрь"] = "12"
        };

        public object GetBackupingItems(string month, string path)
        {
            //string date_file_pattern = string.Concat("\\d{2}\\.", month, "\\.", current_date.Year, "\\.", item_type, "$");   // в запросе !!! другой паттерн
            //string full_pattern = string.Concat(Rgx_pattern, date_file_pattern);

            DirectoryInfo dir = new(path);
            var file_list = dir.GetFiles(item_type);





        }

        //delegate string MonthPattern()
    }






}

