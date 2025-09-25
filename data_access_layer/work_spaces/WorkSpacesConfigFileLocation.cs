// * Файл "WorkSpacesConfigFileLocation.cs": структура для создания пути файла настройки рабочих директорий. *

struct WorkSpacesConfigFileLocation
{
    // По умолчанию, создается в расположении исполняемого файла программы.

    // Пренебрегаем обработкой исключения "UnauthorizedAccessException". Оно возникает при попытке создать этот файл, в заблокированном расположении.

    public static string full_program_path = Path.Combine(Directory.GetCurrentDirectory(), "drives_config.xml");
}
