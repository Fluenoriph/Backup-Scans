using DrivesControl;
using Microsoft.Win32;


string SETTINGS_REG_KEY = "Software\\Ivan_Bogdanov\\Backup_Scans";

string SOURCE_DIR = "C:\\Users\\Asus machine\\Desktop\\Files\\сканы";

string DESTINATION_DIR = "C:\\Users\\Asus machine\\Desktop\\Files\\result_test";

//WorkPath source = new() { Name = SOURCE_DIR };
//Console.WriteLine(source.state);

SettingsCheck directoryCheck = new(SETTINGS_REG_PATH);
directoryCheck.Validate();


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

    struct WorkPath
    {
        private string name;         // null ???
        public PathState status;

        public string Name
        {
            set   // доступ ????
            {
                if (Directory.Exists(value))
                {
                    name = value;
                    status = PathState.INSTALLED;
                    Console.WriteLine("Dir set OK !");
                }
                else
                {
                    status = PathState.DOES_NOT_EXIST;
                    Console.WriteLine("Dir not exist !");
                }
            }
            get { return name; }
        }
    }

    class SettingsCheck(string? path)
    {
        //RegistryKey set_key = Registry.CurrentUser.CreateSubKey(path);

        private static string[] drive_keys = ["source", "destination"];
        //Range drives_range = 0..drive_keys.Length;
        private string default_drive = "None Path";
                
        public WorkPath[] drives = new WorkPath[drive_keys.Length];

        public void Validate()   // refactor name ????
        {
            for (int i = 0; i < drive_keys.Length; i++)
            {
                string? dir_name = (string?)Registry.GetValue(path, drive_keys[i], default_drive);

                if (dir_name != null)
                {
                    drives[i].Name = dir_name;
                    Console.WriteLine(dir_name);
                }

                //Registry.SetValue(registry_path, "source_dir", RegistryValueKind.String);
            }
        }
        public void Set_Path(string path)
        {


        }
    }
}

