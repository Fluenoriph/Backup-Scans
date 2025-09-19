using InfoOut;
using ResultLogOut;


abstract class BaseLoggerControl(List<DirectoryInfo> work_drives)
{
    // Инициализация лог файлов.

    protected readonly MonthLogFile self_obj_month_log_file_in = new(Path.Combine(work_drives[2].FullName, LogFilesNames.MONTH_LOG_FILE));

    protected readonly YearLogFile self_obj_year_log_file_in = new(Path.Combine(work_drives[2].FullName, LogFilesNames.YEAR_LOG_FILE));

    // Вывод отчета в консоль.

    protected FullLogPrinter? self_obj_log_show_in;
}


class MonthLoggerControl : BaseLoggerControl
{
    public MonthLoggerControl(List<DirectoryInfo> work_drives, string month) : base(work_drives)
    {
        MonthBackupProcess self_obj_backup_per_month_lcl = new(work_drives[0], work_drives[1], month);

        if (self_obj_backup_per_month_lcl.Backup_status_in == BackupingStatusCode.BACKUP_SUCCESS)
        {
            // Сообщение об успешном копировании.

            Console.Clear();
            BackupInfo.ShowResult();
            GeneralInfo.ShowStarLine();
            Console.WriteLine('\n');

            // Лог в xml файл.

            XmlMonthLogger self_obj_month_logger_lcl = new(self_obj_month_log_file_in, month,
                                                           self_obj_backup_per_month_lcl.Self_Obj_Sums_in,
                                                           self_obj_backup_per_month_lcl.Self_Obj_Names_Computing_in);

            // Вывод отчета в консоль.

            BackupInfo.ShowLogHeader(month);
            self_obj_log_show_in = new(self_obj_backup_per_month_lcl.Self_Obj_Sums_in.All_Protocols_Sums_in, self_obj_backup_per_month_lcl.Self_Obj_Sums_in.Simple_Protocols_Sums_in);
            self_obj_log_show_in.ShowLog();

            // Если копировали за декабрь, то подводим итоги года, рассчетом сумм всех месяцев из их логов.

            if (month == PeriodsNames.MONTHES[PeriodsNames.DECEMBER_INDEX])
            {
                TotalLogSumsToYearCalculator self_obj_year_calc_result_lcl = new(self_obj_month_log_file_in!, self_obj_year_log_file_in!);

                Console.WriteLine('\n');
                GeneralInfo.ShowLine();
                Console.WriteLine('\n');

                // Выводим отчет за год в консоль.

                BackupInfo.ShowLogHeader(CurrentDate.Current_Year_Print_in);

                var year_sums_lcl = self_obj_year_calc_result_lcl.GetYearSums();
                                
                self_obj_log_show_in.All_Protocol_Sums_in = year_sums_lcl.Item1;
                self_obj_log_show_in.Simple_Protocol_Sums_in = year_sums_lcl.Item2;

                self_obj_log_show_in.ShowLog();
            }

            //HTMLLogger hTMLLogger = new(self_obj_month_logger_lcl.Sums_Sector_in);

            //string.Join("", log_data)

        }
        else if (self_obj_backup_per_month_lcl.Backup_status_in == BackupingStatusCode.BACKUP_FAILURE)
        {
            BackupInfo.ShowCopyError();
        }
        else
        {
            BackupInfo.ShowScansNotFound(month);
        }
    }
}


class YearLoggerControl : BaseLoggerControl
{
    public YearLoggerControl(List<DirectoryInfo> work_drives) : base(work_drives)
    {
        YearBackupProcess self_obj_backup_per_year_lcl = new(work_drives[0], work_drives[1]);

        if (self_obj_backup_per_year_lcl.Backup_status_in == BackupingStatusCode.BACKUP_SUCCESS)
        {
            // Сообщение об успешном копировании.

            Console.Clear();
            BackupInfo.ShowResult();
            GeneralInfo.ShowStarLine();

            // Логгинг года в xml.

            XmlYearLogger self_obj_year_logger_lcl = new(self_obj_year_log_file_in, self_obj_backup_per_year_lcl.Main_Sums_in, 
                                                      self_obj_backup_per_year_lcl.Simple_Protocols_Sums_in);

            // Логгинг каждого месяца.

            foreach (var month_item in self_obj_backup_per_year_lcl.Full_Log_Data_in)
            {
                XmlMonthLogger self_obj_month_logger_lcl = new(self_obj_month_log_file_in, month_item.Item1, month_item.Item2, month_item.Item3);

                BackupInfo.ShowMonthBackupResult(month_item.Item1, month_item.Item2.All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[0]]);
                GeneralInfo.ShowLine();
            }

            // Вывод отчета за год.

            Console.WriteLine('\n');
            BackupInfo.ShowLogHeader(CurrentDate.Current_Year_Print_in);
            self_obj_log_show_in = new(self_obj_backup_per_year_lcl.Main_Sums_in, self_obj_backup_per_year_lcl.Simple_Protocols_Sums_in);
            self_obj_log_show_in.ShowLog();
        }
        else if (self_obj_backup_per_year_lcl.Backup_status_in == BackupingStatusCode.BACKUP_FAILURE)
        {
            BackupInfo.ShowCopyError();
        }
        else
        {
            BackupInfo.ShowScansNotFound(CurrentDate.Current_Year_Print_in);
        }
    }
}
             