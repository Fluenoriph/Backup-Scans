using System.Text.RegularExpressions;
using BackupBlock;
using DrivesControl;
using Microsoft.Win32;


const string SETTINGS_FULL_KEY = "HKEY_CURRENT_USER\\Software\\Ivan_Bogdanov\\Backup_Scans";

string simple_file_pattern = "^\\d{1,4}-(ф|фа|р|ра|м|ма)-";

//const string SOURCE_DIR = "C:\\Users\\Asus machine\\Desktop\\Files\\сканы";
//const string DESTINATION_DIR = "C:\\Users\\Asus machine\\Desktop\\Files\\result_test";


SettingsInWinRegistry x = new(SETTINGS_FULL_KEY);
var dirs = x.GetSettings();

BackupItem y = new(simple_file_pattern);

FileInfo[] mass_files = y.GetBackupingItems("Январь", dirs[0]);

foreach (FileInfo file in mass_files)
{
    Console.WriteLine(file.FullName);
}

Console.WriteLine(y.Items_count);

ProtocolTypes z = new(mass_files);
//z.CalcProtocolTypes();

//Console.WriteLine();


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
    struct MonthValues
    {   
        public Dictionary<string, string> Table { get; set; } = table;

        private static readonly Dictionary<string, string> table = new() 
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

        public MonthValues() {}
    }


    class BackupItem(string rgx_pattern)
    {
        private static DateTime current_date = DateTime.Now;
        private static string Year = current_date.Year.ToString();
        
        public string Item_type { get; set; } = "*.pdf"; 
        private string Rgx_pattern { get; set; } = rgx_pattern;         
        public int Items_count { get; set; }

        private static MonthValues monthes = new();

        public FileInfo[] GetBackupingItems(string month, string path)
        {                        
            string date_file_pattern = string.Concat("\\d{2}\\.", monthes.Table[month], "\\.", Year, "\\.", Item_type, "$");   
            string full_pattern = string.Concat(Rgx_pattern, date_file_pattern);

            Regex rgx = new(full_pattern, RegexOptions.IgnoreCase);

            DirectoryInfo dir = new(path);
            var file_list = dir.GetFiles(Item_type);

            var query = from file in file_list
                        where rgx.IsMatch(file.Name)
                        orderby(file.Name)
                        select file;

            var new_files = query.ToArray();
            Items_count = new_files.Length;

            return new_files;     // status ?  or null files !!!
        }         
    }

    
    class ProtocolTypes(FileInfo[] files)
    {
        private List<string> Files
        {
            set
            {
                foreach (FileInfo file in files)
                {
                    Files.Add(file.Name);
                }
            }
            get { return Files; }
        }
              
        private static string[] protocol_types = ["ф", "фа", "р", "ра", "м", "ма"];   // ключи которые без "а" считать в Уссурийск

        public Dictionary<string, int> type_sums = [];
        public List<string> missing_protocols = [];

        TypePattern rgx_number = static (x) => new($"^(?<number>\\d+)-{x}-", RegexOptions.IgnoreCase);

        public void CalcProtocolTypes()
        {
            foreach (string i in protocol_types)
            {
                Regex rgx = rgx_number(i);

                var query = from file_name in Files
                            where rgx.IsMatch(file_name)
                            select file_name;

                //Console.WriteLine(query.ToArray());
                
            }
        }

        delegate Regex TypePattern(string protocol_type);
    }



}


namespace Tracing
{


}

