using InfoOut;
using System.Globalization;
using System.Security;


class WorkLocations
{
    public List<DrivesConfiguration> Drives { get; } = [];

    public WorkLocations()
    {
        // Установка директорий.

        foreach (string drive_type in XmlTags.DRIVE_TAGS)
        {
            DrivesConfiguration drive = new(drive_type);

            // Вывод информации.

            WorkDirectoriesInfo.ShowDirectorySetupTrue(drive_type, drive.Work_Directory_in!);

            Drives.Add(drive);
        }   
    }

    public List<DirectoryInfo> GetWorkDrives()
    {
        List<DirectoryInfo> work_drives_lcl = [];

        // Инициализация директорий "дисков".

        try
        {
            DirectoryInfo source_lcl = new(Drives[0].Work_Directory_in!);
            work_drives_lcl.Add(source_lcl);

            DirectoryInfo destination_lcl = new(Drives[1].Work_Directory_in!);
            
            // Принудительная проверка на существование папки года для резервного хранилища.

            destination_lcl = destination_lcl.CreateSubdirectory(CurrentDate.Year_in.ToString(CultureInfo.CurrentCulture));
            work_drives_lcl.Add(destination_lcl);

            DirectoryInfo main_log_lcl = new(Drives[2].Work_Directory_in!);
            
            // Принудительная проверка на существование папки года для отчетов.

            main_log_lcl = main_log_lcl.CreateSubdirectory(CurrentDate.Year_in.ToString(CultureInfo.CurrentCulture));

            // Создание папки html отчетов.

            var html_out_log_lcl = main_log_lcl.CreateSubdirectory("html_out");

            work_drives_lcl.Add(main_log_lcl);
            work_drives_lcl.Add(html_out_log_lcl);
        }
        catch (IOException error)
        {
            _ = new ProgramShutDown(ErrorCode.DRIVE_RESOURCE_UNAVAILABLE, error.Message);
        }
        catch (SecurityException error)
        {
            _ = new ProgramShutDown(ErrorCode.DRIVE_RESOURCE_ACCESS_ERROR, error.Message);
        }

        return work_drives_lcl;
    }
}
