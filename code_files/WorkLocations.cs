using InfoOut;
using System.Globalization;
using System.Security;


class WorkLocations
{


    public DirectoryInfo? Log_in { get; }
    public DirectoryInfo? Source_in { get; }
    public DirectoryInfo? Destination_in { get; }

    public WorkLocations()
    {
        // Установка директорий.

        DrivesConfiguration source = new(XmlTags.DRIVE_TAGS[0]);
        WorkDirectoriesInfo.ShowDirectorySetupTrue(XmlTags.DRIVE_TAGS[0], source.Work_Directory_in!);
        
        DrivesConfiguration destination = new(XmlTags.DRIVE_TAGS[1]);
        WorkDirectoriesInfo.ShowDirectorySetupTrue(XmlTags.DRIVE_TAGS[1], destination.Work_Directory_in!);

        DrivesConfiguration log = new(XmlTags.DRIVE_TAGS[2]);
        WorkDirectoriesInfo.ShowDirectorySetupTrue(XmlTags.DRIVE_TAGS[2], log.Work_Directory_in!);

        // Инициализация директорий "дисков".

        try
        {
            Source_in = new(source.Work_Directory_in!);

            Destination_in = new(destination.Work_Directory_in!);

            // Принудительная проверка на существование папки года для резервного хранилища.

            Destination_in = Destination_in.CreateSubdirectory(CurrentDate.Year_in.ToString(CultureInfo.CurrentCulture));

            Log_in = new(log.Work_Directory_in!);

            // Принудительная проверка на существование папки года для отчетов.

            Log_in = Log_in.CreateSubdirectory(CurrentDate.Year_in.ToString(CultureInfo.CurrentCulture));
        }
        catch (IOException error)
        {
            _ = new ProgramShutDown(ErrorCode.DRIVE_RESOURCE_UNAVAILABLE, error.Message);
        }
        catch (SecurityException error)
        {
            _ = new ProgramShutDown(ErrorCode.DRIVE_RESOURCE_ACCESS_ERROR, error.Message);
        }


        // in logger control

        // Инициализация лог файлов.

        Self_Obj_Month_Log_File_in = new(Path.Combine(Log_in!.FullName, LogFilesNames.MONTH_LOG_FILE));

        Self_Obj_Year_Log_File_in = new(Path.Combine(Log_in!.FullName, LogFilesNames.YEAR_LOG_FILE));
    }

    public void Change(string drive_type)
    {



    }
}
