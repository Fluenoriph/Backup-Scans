using System.Xml.Linq;
using DrivesControl;
using Logging;


namespace Tracing
{
    struct RgxMaskConfiguration
    {
        public const string simple_file_pattern = "^\\d{1,4}-(ф|фа|р|ра|м|ма)-";
        public const string eias_file_pattern = "^\\d{5}-\\d{2}-\\d{2}-";
    }
    

    class BackupProcess
    {
        //RegKeyInXML RegKey = new();
        SettingsInWinRegistry Reg_settings = new();
        List<Drive> Drives = new(2);
                
        public void PrepareToBackup()
        {
            Reg_settings.Key = RegKeyInXML.GetPath();

            if (Reg_settings.Key != null)
            {
                bool ready_status;

                do
                {
                    Reg_settings.CheckAndGetDrivesSettings();

                    if (Reg_settings.Key_Status == SettingsStatus.ROOT_DOES_NOT_EXIST)
                    {
                        ready_status = false;

                        IO_Console.Out_info($"\n{Reg_settings.Key} - ключ реестра не существует!\nВведите верный путь:");
                        string? s = IO_Console.Enter_value();
                        //RegKeyInXML.SetPath();
                        continue;
                    }

                    else if (Reg_settings.Drive_Status[0] == SettingsStatus.DRIVE_ERROR)
                    {
                        ready_status= false;

                        IO_Console.Out_info($"\nОшибка чтения настроек исходного диска!\nНастройте путь:");
                        string? s = IO_Console.Enter_value();
                        Reg_settings.SetupDrive(Reg_settings.Drives_names[0], s);
                        continue;
                    }

                    else if (Reg_settings.Drive_Status[1] == SettingsStatus.DRIVE_ERROR)
                    {
                        ready_status = false;

                        IO_Console.Out_info($"\nОшибка чтения настроек резервного диска!\nНастройте путь:");
                        string? s = IO_Console.Enter_value();
                        Reg_settings.SetupDrive(Reg_settings.Drives_names[1], s);
                        continue;
                    }

                    else 
                    {
                        for (int i = 0; i < Reg_settings.Dirs.Count; i++) // recurse func
                        {
                            var d = Drives[i];
                            d.Path = Reg_settings.Dirs[i];

                            if (d.Status == PathState.DOES_NOT_EXIST) 
                            { 
                                ready_status = false;
                                IO_Console.Out_info($"\nДиректория диска:{Reg_settings.Drives_names[i]} не существует! Настройте путь:");
                                string? s = IO_Console.Enter_value();
                                Reg_settings.SetupDrive(Reg_settings.Drives_names[i], s);
                                continue;
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

        
                
    }


    class BlockAnalysis
    {


    }

}
