using DrivesControl;

namespace Tracing
{
    struct InternalConfiguration
    {
        public const string SETTINGS_FULL_KEY = "HKEY_CURRENT_USER\\Software\\Ivan_Bogdanov\\Backup_Scans";
        public const string simple_file_pattern = "^\\d{1,4}-(ф|фа|р|ра|м|ма)-";
        public const string eias_file_pattern = "^\\d{5}-\\d{2}-\\d{2}-";
    }
    

    class BackupProcess
    {
        SettingsInWinRegistry reg_settings = new(InternalConfiguration.SETTINGS_FULL_KEY);  
        Drive source_drive = new();
        Drive destination_drive = new();

        /*RegKeyVerify check_key = () =>
        {

        }*/

        public void DriveVerify()
        {
            do
            {




            }
            while ();

        }

        delegate void RegKeyVerify();
    }


    class BlockAnalysis
    {


    }

}
