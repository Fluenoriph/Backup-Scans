// * Файл "YearLoggerControl.cs": вариант выполнения программы за год. *

class YearLoggerControl : BaseAppControl
{
    public YearLoggerControl(List<DirectoryInfo> work_spaces) : base(work_spaces)
    {
        // Запуск копирования.

        YearBackupProcess self_obj_backup_per_year_lcl = new(work_spaces[0], work_spaces[1]);

        if (self_obj_backup_per_year_lcl.Backup_status_in == BackupingStatusCode.BACKUP_SUCCESS)
        {
            PrintCopySuccessfull();

            // Лог в XML файл.

            _ = new XMLYearLogger(self_obj_year_log_file_in, self_obj_backup_per_year_lcl.Main_Sums_in,
                                  self_obj_backup_per_year_lcl.Simple_Protocols_Sums_in);

            // Логгинг каждого месяца.

            foreach (var month_item in self_obj_backup_per_year_lcl.Full_Log_Data_in)
            {
                _ = new XMLMonthLogger(self_obj_month_log_file_in, month_item.Item1, month_item.Item2, month_item.Item3);

                // Вывод в консоль всех скопированных файлов по каждому месяцу.

                BackupInfo.ShowMonthBackupResult(month_item.Item1, month_item.Item2.Main_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[0]]);
                GeneralInfo.ShowLine();
            }








            // Вывод отчета за год в консоль.

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
