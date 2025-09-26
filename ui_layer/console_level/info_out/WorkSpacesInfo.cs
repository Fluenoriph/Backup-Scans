// * Файл "WorkSpacesInfo.cs": класс для вывода информации, связанной с рабочими пространствами. *

class WorkSpacesInfo
{
    // Кириллические названия "дисков". 

    struct WorkDirectories
    {
        public static Dictionary<string, string> NAMES = new()
        {
            [XMLWorkSpacesTags.SPACE_TYPE[0]] = "ИСХОДНАЯ",
            [XMLWorkSpacesTags.SPACE_TYPE[1]] = "РЕЗЕРВНАЯ",
            [XMLWorkSpacesTags.SPACE_TYPE[2]] = "ОТЧЕТНАЯ"
        };
    }

    const string DIR_EXAMPLE = "Пример: C:\\Folder\\Subfolder";

    // * Сообщение о положительной установке директории. *

    public static void ShowDirectorySetupTrue(string work_space_type, string directory)
    {
        Console.WriteLine($" {ConsoleSymbols.STAR} {WorkDirectories.NAMES[work_space_type]} директория: {directory}\n");
    }

    // * Сообщение об отрицательной установке директории. *

    public static void ShowDirectoryExistFalse(string work_space_type, string directory)
    {
        if (directory is "")
        {
            Console.WriteLine($" {ConsoleSymbols.GRILLE} {WorkDirectories.NAMES[work_space_type]} директория не установлена, необходимо установить.\n");
        }
        else
        {
            Console.WriteLine($" {ConsoleSymbols.GRILLE} {WorkDirectories.NAMES[work_space_type]} директория [{directory}] не существует, установите правильную !\n");
        }
    }

    // * Сообщение о том, что директория установлена. *

    public static void ShowInstallDirectory(string work_space_type)
    {
        Console.WriteLine($"\n {ConsoleSymbols.GRILLE} {WorkDirectories.NAMES[work_space_type]} директория успешно установлена !");
    }

    // * Сообщение о смене директории. *

    public static void ShowEnterDirectoryType()
    {
        List<string> dir_type_info_lcl = [];

        // Создание параметров для выбора.

        foreach (var work_space in XMLWorkSpacesTags.SPACE_TYPE)
        {
            dir_type_info_lcl.Add(string.Concat(ParameterTemplates.CreateParameterDigit(XMLWorkSpacesTags.SPACE_TYPE, work_space), Symbols.LINE, WorkDirectories.NAMES[work_space]));
        }

        // Сам вывод.

        Console.WriteLine($" {ConsoleSymbols.STAR} Выберите тип директории {ConsoleSymbols.FLOW_RIGHT}\n");
        ParameterTemplates.ShowParameters(dir_type_info_lcl);
        GeneralInfo.ShowLine();
    }

    // * Сообщение о вводе директории. *

    public static void ShowEnterTheDirectory()
    {
        Console.WriteLine($"\n{ConsoleSymbols.FLOW_RIGHT} Введите директорию:\n\n {DIR_EXAMPLE}");
    }
}
