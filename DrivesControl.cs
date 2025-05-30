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
        static List<bool> status = new(2);

        void GetSettings();
        void SetupDrive(string drive_type, string new_path);
    }


    struct Drive
    {
        private string path;
        public PathState Status { get; set; }
        public string Path
        {
            set   
            {
                if (Directory.Exists(value))      
                {
                    path = value;
                    Status = PathState.READY_TO_WORK;
                }
                else { Status = PathState.DOES_NOT_EXIST; }
            }

            get { return Path; }
        }        
    }


    class SettingsInWinRegistry(string key_path) : IDriveConfig
    {
        private string Key { get; set; } = key_path; // not property ??
        public bool Key_Status { get; set; } = true;
        public List<string> Dirs { get; set; } = []; 

        public void GetSettings()
        {
            for (int i = 0; i < IDriveConfig.drives.Count; i++)
            {
                string? dir_name = (string?)Registry.GetValue(Key, IDriveConfig.drives[i], "None");

                if (dir_name == null) { Key_Status = false; }
                
                else if (dir_name == "None") { IDriveConfig.status[i] = false; }
                
                else 
                { 
                    Dirs.Add(dir_name);
                    IDriveConfig.status[i] = true;
                }
            }
        }

        public void SetupRegKey(string reg_key)
        {
            Key = reg_key;
            Key_Status = true;
        }

        public void SetupDrive(string drive_type, string new_path) { 
            Registry.SetValue(Key, drive_type, new_path, RegistryValueKind.String);   // exceptions ????
        }
    }
}
