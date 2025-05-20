using DrivesControl;
using Microsoft.Win32;


string SETTINGS_REG_PATH = "HKEY_CURRENT_USER\\Software\\Ivan_Bogdanov\\Backup_Scans";

string SOURCE_DIR = "C:\\Users\\Asus machine\\Desktop\\Files\\сканы";

string DESTINATION_DIR = "C:\\Users\\Asus machine\\Desktop\\Files\\result_test";

//WorkPath source = new() { Name = SOURCE_DIR };
//Console.WriteLine(source.state);

DirectoryCheck directoryCheck = new(SETTINGS_REG_PATH);
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
        public string name;         // null ???
        public PathState state;

        public string Name
        {
            set   // доступ ????
            {
                if (Directory.Exists(value))
                {
                    name = value;
                    state = PathState.INSTALLED;
                    Console.WriteLine("Dir set OK !");
                }
                else
                {
                    state = PathState.DOES_NOT_EXIST;
                    Console.WriteLine("Dir not exist !");
                }
            }
            get { return name; }
        }
    }

    class DirectoryCheck(string? registry_path)
    {
        private string? registry_path = registry_path;
        private static string[] drive_keys = ["source", "destination"];
        //Range drives_range = 0..drive_keys.Length;
        private string default_drive = "None Path";

        //public WorkPath source = new();
        //public WorkPath destination = new();
        public WorkPath[] drives = new WorkPath[drive_keys.Length];

        public void Validate()
        {
            for (int i = 0; i < drive_keys.Length; i++)
            {
                string? dir_name = (string?)Registry.GetValue(registry_path, drive_keys[i], default_drive);

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

