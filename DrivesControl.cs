using System.Xml.Linq;
using BackupBlock;
using Logging;
using Microsoft.Win32;
using Tracing;


namespace DrivesControl
{
    enum SettingsStatus
    {
        DRIVE_ERROR,
        DRIVE_RECEIVED,

        PATH_DOES_NOT_EXIST,
        PATH_INSTALLED
    }


    interface IDrivesConfiguration
    {
        static List<string> drive_type = ["SOURCE", "DESTINATION"];
        static List<SettingsStatus> drive_status = new(2); // надо ли

        bool GetSettings();
        //bool CheckDrivesDirectories();
        //void SetupDrive(string drive_type, string new_path);
    }


    class Drive(string type)
    {
        private string? directory;
        public string Name { get; } = type;           
        public SettingsStatus Directory_Status { get; private set; }
        public string Directory
        {
            get => directory ?? "NULL_DIRECTORY";  // " "

            set
            {
                if (System.IO.Directory.Exists(value))
                {
                    directory = value;
                    Directory_Status = SettingsStatus.PATH_INSTALLED;
                }
                else 
                { 
                    Directory_Status = SettingsStatus.PATH_DOES_NOT_EXIST; 
                }
            }
        }
    }


    class XMLConfig : IDrivesConfiguration
    {
        private readonly int drives_count = IDrivesConfiguration.drive_type.Count;
        public List<Drive> Drives { get; private set; } =                    
        [
            new(IDrivesConfiguration.drive_type[0]),
            new(IDrivesConfiguration.drive_type[1])
        ];
            
        public bool GetSettings()        // return error codes on upper level !!
        {
            XElement? config = ConfigFiles.GetDrivesConfig();
            
            if (config != null)
            {
                for (int i = 0; i < drives_count; i++)
                {
                    
                        string? path = config.Element(IDrivesConfiguration.drive_type[i])?.Value;
                        
                        if (path != null)
                        {
                            Console.WriteLine(path);

                            Drives[i].Directory = path;
                                                    
                        if (Drives[i].Directory_Status == SettingsStatus.PATH_DOES_NOT_EXIST)
                        {
                                

                                Console.WriteLine($"{Drives[i].Name} path not exist\nSetup Drive >>");
                                string input_dir = Console.ReadLine();

                            ConfigFiles.SetupDriveDirectory(IDrivesConfiguration.drive_type[i], input_dir);
                            return false;
                                
                            }




                        //Drives[i].Directory = path; 
                        //Drive_Status[i] = SettingsStatus.DRIVE_RECEIVED;
                        //Console.WriteLine("\nPath NULL !!!!");
                        //C:\Users\Asus machine\Desktop\Files\сканы
                        //C:\Users\Asus machine\Desktop\Files\result_test
                    }
                    else
                        {
                            IDrivesConfiguration.drive_status.Insert(i, SettingsStatus.DRIVE_ERROR);
                            Console.WriteLine($"\nError: {IDrivesConfiguration.drive_type[i]} ошибочен в файле конфигурации !! Exit >>>");
                            
                        return false;   // бесконечно
                        }

                    


                }
                return true;
            }
            else
            {
                Console.WriteLine("\nXML config error !! Exit >>>");
                return false;
            }
        }
        
        



    }


         
        

        
}
