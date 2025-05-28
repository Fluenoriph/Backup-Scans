using Microsoft.Win32;


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


    interface IDriveConfig
    {
        static List<string> drives = ["source", "destination"];

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


    class SettingsInWinRegistry(string key_path) : IDriveConfig
    {
        private string Key { get; set; } = key_path;

        public List<string> GetSettings()
        {
            List<string> dirs = [];

            for (int i = 0; i < IDriveConfig.drives.Count; i++)
            {
                string? dir_name = (string?)Registry.GetValue(Key, IDriveConfig.drives[i], "None");

                if (dir_name == null)
                {
                    Console.WriteLine("Reg Key-Path Not Found !!!\n");
                    Console.WriteLine($"Setup Reg Key >>> Set Drive {IDriveConfig.drives[i]} Data !");
                    string? data = Console.ReadLine();
                    SetupDrive(IDriveConfig.drives[i], data);
                }
                else if (dir_name == "None")
                {
                    Console.WriteLine($"None Key !!!\nSetup Drive {IDriveConfig.drives[i]} Value >>>");
                    string? data = Console.ReadLine();
                    SetupDrive(IDriveConfig.drives[i], data);
                }
                else
                {
                    dirs.Add(dir_name);
                }
            }
            return dirs;
        }

        public void SetupDrive(string drive_type, string path) { Registry.SetValue(Key, drive_type, path, RegistryValueKind.String); }
    }
}
