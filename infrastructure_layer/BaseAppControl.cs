// * Файл "BaseAppControl.cs": базовый класс для вариантов выполнения программы. *

abstract class BaseAppControl(List<DirectoryInfo> work_spaces)
{
    // Инициализация лог файлов.

    protected readonly XMLMonthLogFile self_obj_month_log_file_in = new(Path.Combine(work_spaces[2].FullName, LogFilesNames.MONTH_LOG_FILE));

    protected readonly XMLYearLogFile self_obj_year_log_file_in = new(Path.Combine(work_spaces[2].FullName, LogFilesNames.YEAR_LOG_FILE));

    // Вывод отчета в консоль.

    protected FullLogPrinter? self_obj_log_show_in;

    // Сообщение об успешном копировании.

    protected static void PrintCopySuccessfull()
    {
        Console.Clear();
        BackupInfo.ShowResult();
        GeneralInfo.ShowStarLine();
        Console.WriteLine('\n');
    }
}
