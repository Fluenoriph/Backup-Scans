// * Файл "GeneralInfo.cs": класс для вывода основной информации. *

class GeneralInfo
{
    static readonly string star_line = new(ConsoleSymbols.STAR, 60);

    // * Показать линию (-----). *

    public static void ShowLine()
    {
        Console.WriteLine(new string(Symbols.LINE, 60));
    }

    // * Показать линию из звездочек (*******). *

    public static void ShowStarLine()
    {
        Console.WriteLine(star_line);
    }

    // * Информация об авторе программы. *

    public static void ShowAuthorInfo()
    {
        Console.WriteLine("\n [ Иван Богданов. Все права защищены. 2025 г. ]\n");
    }

    // * Показать информацию о программе. *

    public static void ShowProgramInfo()
    {
        Console.WriteLine("\n >> Backup \"PDF\" Protocols Scan Files v.2.1 <<\n");
    }

    // * Показать главное меню программы. *

    public static void ShowProgramMenu()
    {
        List<string> value_info_lcl = [];

        // Создание списка параметров по периодам.

        foreach (var month in Periods.MONTHES)
        {
            value_info_lcl.Add(string.Concat(month, Symbols.LINE, ParameterTemplates.CreateParameterDigit(Periods.MONTHES, month)));
        }
        value_info_lcl.Add(string.Concat(Periods.YEAR, Symbols.LINE, $"\"{CurrentDate.Year_in}\""));

        // Вывод.

        Console.WriteLine($" {ConsoleSymbols.STAR} МЕНЮ {ConsoleSymbols.STAR}\n");

        Console.WriteLine($"{ConsoleSymbols.FLOW_RIGHT} Для запуска резервного копирования, введите значение периода {ConsoleSymbols.FLOW_RIGHT}\n");
        ParameterTemplates.ShowParameters(value_info_lcl);
        Console.WriteLine('\n');

        Console.WriteLine($"{ConsoleSymbols.FLOW_RIGHT} Чтобы изменить директорию, введите: \"{ConsoleSymbols.CHANGE_DIRECTORY_FUNCTION}\"\n");
        ShowLine();
    }

    // * Сообщение о рестарте или завершении программы. *

    public static bool RestartOrExitProgram()
    {
        Console.WriteLine($"\n\n {new string('/', 3)} Для выхода в главное меню нажмите < ПРОБЕЛ >, чтобы завершить работу программы нажмите любую клавишу {new string('\\', 3)}");

        if (Console.ReadKey(intercept: true).Key is ConsoleKey.Spacebar)
        {
            Console.Clear();

            return true;
        }
        else
        {
            return false;
        }
    }
}
