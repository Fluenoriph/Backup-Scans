using System.Xml.Linq;
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
        static List<SettingsStatus> drive_status = new(2); // надо ли

        void GetDrivesSettings();
        void PrepareToBackup();
        void SetupDrive(string drive_type, string new_path);
    }


    class Drive(string type)
    {
        public string Name { get; } = type;
        private string directory;                 // nullable warning off ??
        public SettingsStatus Directory_Status { get; set; }
        public string Directory
        {
            set
            {
                if (System.IO.Directory.Exists(value))
                {
                    directory = value;
                    Directory_Status = SettingsStatus.PATH_INSTALLED;
                }

                else { Directory_Status = SettingsStatus.PATH_DOES_NOT_EXIST; }
            }

            get => directory;
        }
    }


    class XMLConfig : IDrivesConfiguration
    {
        private List<SettingsStatus> Drive_Status { get; set; } = [];
        private readonly int drives_count = IDrivesConfiguration.drive_type.Count;
        
        public List<Drive> Drives 
        {     
            set
            {
                for (int i = 0; i < drives_count; i++)
                {
                    Drive drive = new(IDrivesConfiguration.drive_type[i]);       
                    value.Insert(i, drive);                    
                }
            }

            get { return Drives; }
        }
                              
        private void GetDrivesSettings()
        {
            XElement? config = ConfigFiles.Xml_drives_config.Element("configuration");    // xml error  test xml 
            
            
            string? path = config?.Element("reg_key_path")?.Value;     // null ?. test

            Console.WriteLine(path);
                        
            if (Key is not null) 
            { 
                Key_Status = SettingsStatus.ROOT_KEY_INSTALLED; 
            }

            else 
            { 
                Key_Status = SettingsStatus.ROOT_KEY_READ_FAILED;
                
                /// check !!!
            }
        }
                
               
        
        public void PrepareToBackup()
        {
            GetRegistryKeyIntoXML();

            if (Key_Status == SettingsStatus.ROOT_KEY_INSTALLED)
            {                
                bool ready_status = false;

                do
                {
                    GetDrivesSettings();

                    if (Key_Status == SettingsStatus.ROOT_KEY_DOES_NOT_EXIST)
                    {
                        //ready_status = false;

                        IO_Console.Out_info($"\n{Key} - ключ реестра не существует!\nВведите верный путь:");
                        string? s = IO_Console.Enter_value();

                        // setup new settings !!! method
                        //RegKeyInXML.SetPath();
                        //continue;
                    }

                    else
                    {
                        for (int i = 0; i < drives_count; i++)
                        {
                            if (Drive_Status[i] == SettingsStatus.DRIVE_ERROR)
                            {
                                //ready_status = false;
                                Console.WriteLine($"\nОшибка чтения настроек диска {IDrivesConfiguration.drive_type[i]}\nНастройте директорию:");
                                string? s = Console.ReadLine();
                                SetupDrive(IDrivesConfiguration.drive_type[i], s);  // auto ??
                            }
                            
                            else
                            {
                                if (Drives[i].Directory_Status == SettingsStatus.PATH_DOES_NOT_EXIST)
                                {
                                    Console.WriteLine($"\n{Drives[i].Name} диска путь не существует");
                                }
                                else
                                {
                                    ready_status = true;
                                }
                            }
                        }

                                                
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







        public void SetupDrive(string drive_type, string new_path) { 
            Registry.SetValue(Key, drive_type, new_path, RegistryValueKind.String);   // exceptions ????
        }
    }


         
        

        
    }
