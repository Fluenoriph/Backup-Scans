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


    enum SettingsStatus
    {
        ROOT_DOES_NOT_EXIST,  // reg key error
        ROOT_OK,
        DRIVE_ERROR,
        DRIVE_RECEIVED
    }


    interface IDriveConfig
    {
        static List<string> drives = ["SOURCE", "DESTINATION"];
        static List<SettingsStatus> drive_status = new(2);

        void CheckAndGetDrivesSettings();
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

            readonly get => path;
        }
    }


    class SettingsInWinRegistry : IDriveConfig
    {
        public string? Key { get; set; } // not property ??
        public SettingsStatus Key_Status { get; set; } = SettingsStatus.ROOT_OK;
        public List<string> Drives_names { get; set; } = IDriveConfig.drives;
        public List<SettingsStatus> Drive_Status { get; set; } = IDriveConfig.drive_status;
        public List<string> Dirs { get; set; } = new(2); 

        public void CheckAndGetDrivesSettings()
        {
            for (int i = 0; i < Drives_names.Count; i++)
            {
                string? dir_name = (string?)Registry.GetValue(Key, Drives_names[i], "None");

                if (dir_name == null) { Key_Status = SettingsStatus.ROOT_DOES_NOT_EXIST; }
                
                else if (dir_name == "None") { Drive_Status.Insert(i, SettingsStatus.DRIVE_ERROR); }
                
                else 
                { 
                    Dirs.Add(dir_name);
                    Drive_Status.Insert(i, SettingsStatus.DRIVE_RECEIVED);
                }
            }
        }

        /*public void SetupRegKey(string reg_key)
        {
            Key = reg_key;
            Key_Status = SettingsStatus.ROOT_OK;
        }*/

        public void SetupDrive(string drive_type, string new_path) { 
            Registry.SetValue(Key, drive_type, new_path, RegistryValueKind.String);   // exceptions ????
        }
    }
}
