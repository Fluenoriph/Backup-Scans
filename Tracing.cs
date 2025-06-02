using System.Xml.Linq;
using DrivesControl;

namespace Tracing
{
    struct InternalConfiguration
    {
        public const string SETTINGS_FULL_KEY = "HKEY_CURRENT_USER\\Software\\Ivan_Bogdanov\\Backup_Scan";
        public const string simple_file_pattern = "^\\d{1,4}-(ф|фа|р|ра|м|ма)-";
        public const string eias_file_pattern = "^\\d{5}-\\d{2}-\\d{2}-";
    }
    

    class BackupProcess
    {
        SettingsInWinRegistry reg_settings = new(InternalConfiguration.SETTINGS_FULL_KEY);  
        Drive source_drive = new();
        Drive destination_drive = new();
                
        public void PrepareToBackup() // setup ??? return true or false
        {
            reg_settings.CheckAndGetDrivesSettings();

            if (reg_settings.Key_Status == SettingsStatus.ROOT_DOES_NOT_EXIST)
            {
                Console.WriteLine("Key Failed\nEnter new key:");
                string key = Console.ReadLine();
                reg_settings.SetupRegKey(key);
                    //break;
            }

            else if (reg_settings.Drive_Status[0] == SettingsStatus.DRIVE_ERROR)
            {
                Console.WriteLine("Source drive error\nEnter dir:");
                string path = Console.ReadLine();
                reg_settings.SetupDrive(reg_settings.Drives[0], path);
                     //break;
            }

            else if (reg_settings.Drive_Status[1] == SettingsStatus.DRIVE_ERROR)
            {
                Console.WriteLine("Destination drive error\nEnter dir:");
                string path = Console.ReadLine();
                reg_settings.SetupDrive(reg_settings.Drives[1], path);
                    //break;
            }
        }

                //delegate void RegKeyVerify();
    }


    class BlockAnalysis
    {


    }

}
