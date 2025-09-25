// * Файл "WorkDirectoriesInfo.cs": класс для вывода информации, связанной с рабочими пространствами. *

class WorkDirectoriesInfo
{
    // Кириллические названия "дисков". 

    struct WorkDirectories
    {
        public static Dictionary<string, string> NAMES = new()
        {
            [XMLLogTags.DRIVE_TAGS[0]] = "ИСХОДНАЯ",
            [XMLLogTags.DRIVE_TAGS[1]] = "РЕЗЕРВНАЯ",
            [XMLLogTags.DRIVE_TAGS[2]] = "ОТЧЕТНАЯ"
        };
    }

    const string DIR_EXAMPLE = "Пример: C:\\Folder\\Subfolder";

    // * Сообщение о положительной установке директории. *

    public static void ShowDirectorySetupTrue(string drive_type, string directory)
    {
        Console.WriteLine($" {Symbols.STAR} {WorkDirectories.NAMES[drive_type]} директория: {directory}\n");
    }

    // * Сообщение об отрицательной установке директории. *

    public static void ShowDirectoryExistFalse(string drive_type, string directory)
    {
        if (directory is "")
        {
            Console.WriteLine($" {Symbols.GRILLE} {WorkDirectories.NAMES[drive_type]} директория не установлена, необходимо установить.\n");
        }
        else
        {
            Console.WriteLine($" {Symbols.GRILLE} {WorkDirectories.NAMES[drive_type]} директория [{directory}] не существует, установите правильную !\n");
        }
    }

    // * Сообщение о том, что директория установлена. *

    public static void ShowInstallDirectory(string drive_type)
    {
        Console.WriteLine($"\n {Symbols.GRILLE} {WorkDirectories.NAMES[drive_type]} директория успешно установлена !");
    }

    // * Сообщение о смене директории. *

    public static void ShowEnterDirectoryType()
    {
        List<string> dir_type_info_lcl = [];

        // Создание параметров для выбора.

        foreach (var drive in XMLLogTags.DRIVE_TAGS)
        {
            dir_type_info_lcl.Add(string.Concat(ParameterTemplates.CreateParameterDigit(XMLLogTags.DRIVE_TAGS, drive), Symbols.LINE, WorkDirectories.NAMES[drive]));
        }

        // Сам вывод.

        Console.WriteLine($" {Symbols.STAR} Выберите тип директории {Symbols.FLOW_RIGHT}\n");
        ParameterTemplates.ShowParameters(dir_type_info_lcl);
        GeneralInfo.ShowLine();
    }

    // * Сообщение о вводе директории. *

    public static void ShowEnterTheDirectory()
    {
        Console.WriteLine($"\n{Symbols.FLOW_RIGHT} Введите директорию:\n\n {DIR_EXAMPLE}");
    }
}
