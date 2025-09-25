// * 

using ResultLogOut;


abstract class BaseAppControl(List<DirectoryInfo> work_drives)
{
    // Инициализация лог файлов.

    protected readonly MonthLogFile self_obj_month_log_file_in = new(Path.Combine(work_drives[2].FullName, LogFilesNames.MONTH_LOG_FILE));

    protected readonly YearLogFile self_obj_year_log_file_in = new(Path.Combine(work_drives[2].FullName, LogFilesNames.YEAR_LOG_FILE));

    // Вывод отчета в консоль.

    protected FullLogPrinter? self_obj_log_show_in;
}
