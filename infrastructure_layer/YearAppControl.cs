// * 

using InfoOut;


class YearLoggerControl : BaseAppControl
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
