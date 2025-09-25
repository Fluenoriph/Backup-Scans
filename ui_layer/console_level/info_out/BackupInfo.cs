// * Файл "BackupInfo.cs": класс для вывода информации, связанной с резервным копированием. *

class BackupInfo
{
    // * Сообщение о том, что протоколы не найдены. *

    public static void ShowScansNotFound(string period)
    {
        Console.WriteLine($"\n {Symbols.GRILLE} За {period} сканов не найдено !");
    }

    // * Заголовок отчета. *

    public static void ShowLogHeader(string period)
    {
        string borders_lcl = new(Symbols.STAR, 3);
        Console.WriteLine($" {borders_lcl} Отчет за {period} {borders_lcl}");
    }

    // * Положительный результат. *

    public static void ShowResult()
    {
        Console.WriteLine($"\n {Symbols.GRILLE} Резервное копирование завершено успешно !\n");
    }

    // * Вывод результата количества скопированных файлов за месяц. *

    public static void ShowMonthBackupResult(string period, int file_count)
    {
        Console.WriteLine($" {Symbols.STAR} За {period} успешно скопировано < {file_count} > файл(ов)");
    }

    // * Сообщение об ошибке копирования. *

    public static void ShowCopyError()
    {
        Console.WriteLine($" {Symbols.GRILLE} Критическая ошибка копирования файлов !\n   Перезапустите программу !");
    }

    // * Сообщение об ожидании. *

    public static void ShowVisualWait()
    {
        Console.Write($"\n {Symbols.GRILLE} Выполняется копирование. Ожидайте ...\n\n");
    }
}
