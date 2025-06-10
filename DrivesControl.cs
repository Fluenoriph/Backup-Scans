using BackupBlock;
using Logging;
using Microsoft.Win32;
using Tracing;


namespace DrivesControl
{
    enum SettingsStatus
    {
        ROOT_KEY_DOES_NOT_EXIST,
        ROOT_KEY_INSTALLED,
        ROOT_KEY_READ_FAILED,
        DRIVE_ERROR,
        DRIVE_RECEIVED,
        PATH_DOES_NOT_EXIST,
        PATH_INSTALLED
    }


    interface IDrivesConfiguration
    {
        static List<string> drive_type = ["SOURCE", "DESTINATION"];
        static List<SettingsStatus> drive_status = new(2);

        void CheckAndGetDrivesSettings();
        void PrepareToBackup();
        void SetupDrive(string drive_type, string new_path);
    }


    struct Drive
    {
        private string directory;
        public SettingsStatus Dir_Status { get; set; }
        public string Directory
        {
            set
            {
                if (System.IO.Directory.Exists(value))
                {
                    directory = value;
                    Dir_Status = SettingsStatus.PATH_INSTALLED;
                }
                else { Dir_Status = SettingsStatus.PATH_DOES_NOT_EXIST; }
            }

            readonly get => directory;
        }
    }


    class ConfigurationInWinRegistry : IDrivesConfiguration
    {
        private string? Key { get; set; }
        private SettingsStatus Key_Status { get; set; }



        public List<Drive> Drives = new(2);




        //public List<string> Drives_Names { get; set; } = IDrivesConfiguration.drive_type;
        //public List<SettingsStatus> Drive_Status { get; set; } = IDrivesConfiguration.drive_status;
        public List<string> Dirs { get; set; } = new(2); 

        private void GetRegKey()
        {
            Key = RegKeyInXML.GetPath();

            if (Key is not null) { Key_Status = SettingsStatus.ROOT_KEY_INSTALLED; }

            else { Key_Status = SettingsStatus.ROOT_KEY_READ_FAILED; }
        }


        public void CheckAndGetDrivesSettings()
        {
            
            
            string none_drive_name = "NULL_DRIVE";

            for (int i = 0; i < IDrivesConfiguration.drive_type.Count; i++)
            {
                string? dir_name = (string?)Registry.GetValue(Key, IDrivesConfiguration.drive_type[i], none_drive_name);

                if (dir_name == null) { Key_Status = SettingsStatus.ROOT_KEY_DOES_NOT_EXIST; }
                
                else if (dir_name == none_drive_name) { Drive_Status.Insert(i, SettingsStatus.DRIVE_ERROR); }
                
                else 
                { 
                    Dirs.Add(dir_name);
                    Drive_Status.Insert(i, SettingsStatus.DRIVE_RECEIVED);
                }
            }
        }

        public void SetupDrive(string drive_type, string new_path) { 
            Registry.SetValue(Key, drive_type, new_path, RegistryValueKind.String);   // exceptions ????
        }
    }


    class DrivesConfiguration
    {
        private readonly SettingsInWinRegistry Reg_settings = new();
        

        public void PrepareToBackup()
        {
            Reg_settings.Key = 

            if (Reg_settings.Key != null)
            {
                bool ready_status;

                do
                {
                    Reg_settings.CheckAndGetDrivesSettings();

                    if (Reg_settings.Key_Status == SettingsStatus.ROOT_KEY_DOES_NOT_EXIST)
                    {
                        ready_status = false;

                        IO_Console.Out_info($"\n{Reg_settings.Key} - ключ реестра не существует!\nВведите верный путь:");
                        string? s = IO_Console.Enter_value();
                        //RegKeyInXML.SetPath();
                        //continue;
                    }

                    else if (Reg_settings.Drive_Status[0] == SettingsStatus.DRIVE_ERROR)
                    {
                        ready_status = false;

                        IO_Console.Out_info($"\nОшибка чтения настроек исходного диска!\nНастройте путь:");
                        string? s = IO_Console.Enter_value();
                        Reg_settings.SetupDrive(Reg_settings.Drives_Names[0], s);
                        //continue;
                    }

                    else if (Reg_settings.Drive_Status[1] == SettingsStatus.DRIVE_ERROR)
                    {
                        ready_status = false;

                        IO_Console.Out_info($"\nОшибка чтения настроек резервного диска!\nНастройте путь:");
                        string? s = IO_Console.Enter_value();
                        Reg_settings.SetupDrive(Reg_settings.Drives_Names[1], s);
                        //continue;
                    }

                    else
                    {
                        for (int i = 0; i < Reg_settings.Drives_Names.Count; i++)
                        {
                            Drive d = new()
                            {
                                Directory = Reg_settings.Dirs[i]
                            };
                            Drives.Insert(i, d);

                            if (Drives[i].Status == PathState.DOES_NOT_EXIST)  // Test !!
                            {
                                IO_Console.Out_info($"\nДиректория диска:{Reg_settings.Drives_Names[i]} не существует! Настройте путь:");
                                string? s = IO_Console.Enter_value();
                                Reg_settings.SetupDrive(Reg_settings.Drives_Names[i], s);
                            }
                        }

                        ready_status = true;
                    }
                }
                while (ready_status == false);
            }
            else
            {
                IO_Console.Out_info("\nОшибка чтения файла настроек!\n");
                return;
            }
        }
    }
