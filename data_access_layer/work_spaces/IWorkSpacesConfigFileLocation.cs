// * Файл "WorkSpacesConfigFileLocation.cs": интерфейс для создания пути файла настройки рабочих директорий. *
//   По умолчанию, создается в расположении исполняемого файла программы.

interface IWorkSpacesConfigFileLocation
{
    // Пренебрегаем обработкой исключения "UnauthorizedAccessException". Оно возникает при попытке создать этот файл, в заблокированном расположении.

    static string GetPath()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "drives_config.xml");
    }
}
