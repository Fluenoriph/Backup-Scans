// * Файл "WorkSpacesCreator.cs": класс-конструктор рабочих пространств. * 

using System.Globalization;
using System.Security;


class WorkSpacesCreator
{
    const string HTML_LOGS_LOCATION = "html_out\\logs";
    public List<WorkSpaceSetuper> Spaces_in { get; } = [];

    public WorkSpacesCreator()
    {
        // Установка директорий.

        foreach (string space_type in XMLWorkSpacesTags.SPACE_TYPE)
        {
            WorkSpaceSetuper space = new(space_type);

            // Вывод информации, в данном случае в консоль.

            WorkSpacesInfo.ShowDirectorySetupTrue(space_type, space.Directory_in!);

            Spaces_in.Add(space);
        }   
    }

    public List<DirectoryInfo> GetWorkSpaces()
    {
        List<DirectoryInfo> spaces_lcl = [];

        // Инициализация директорий пространств.

        try
        {
            DirectoryInfo source_lcl = new(Spaces_in[0].Directory_in!);
            spaces_lcl.Add(source_lcl);

            DirectoryInfo destination_lcl = new(Spaces_in[1].Directory_in!);
            
            // Принудительная проверка на существование папки года для резервного хранилища.

            destination_lcl = destination_lcl.CreateSubdirectory(CurrentDate.Year_in.ToString(CultureInfo.CurrentCulture));
            spaces_lcl.Add(destination_lcl);

            DirectoryInfo main_log_lcl = new(Spaces_in[2].Directory_in!);
            
            // Принудительная проверка на существование папки года для отчетов.

            main_log_lcl = main_log_lcl.CreateSubdirectory(CurrentDate.Year_in.ToString(CultureInfo.CurrentCulture));

            // Создание папки html отчетов.

            var html_log_lcl = main_log_lcl.CreateSubdirectory(HTML_LOGS_LOCATION);  

            spaces_lcl.Add(main_log_lcl);
            spaces_lcl.Add(html_log_lcl);
        }
        catch (IOException error)
        {
            _ = new ProgramShutDown(ErrorCode.DRIVE_RESOURCE_UNAVAILABLE, error.Message);
        }
        catch (SecurityException error)
        {
            _ = new ProgramShutDown(ErrorCode.DRIVE_RESOURCE_ACCESS_ERROR, error.Message);
        }

        return spaces_lcl;
    }
}
