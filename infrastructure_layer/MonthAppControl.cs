// * Файл "MonthAppControl.cs": вариант выполнения программы за месяц. *

class MonthAppControl : BaseAppControl
{
    public MonthAppControl(List<DirectoryInfo> work_spaces, string month) : base(work_spaces)
    {
        // Запуск копирования.

        MonthBackupProcess self_obj_backup_per_month_lcl = new(work_spaces[0], work_spaces[1], month);

        if (self_obj_backup_per_month_lcl.Backup_status_in == BackupingStatusCode.BACKUP_SUCCESS)
        {
            PrintCopySuccessfull();

            // Лог в xml файл.

            XMLMonthLogger self_obj_month_logger_lcl = new(self_obj_month_log_file_in, month,
                                                           self_obj_backup_per_month_lcl.Self_Obj_Sums_in,
                                                           self_obj_backup_per_month_lcl.Self_Obj_Names_Computing_in);

            // Создание HTML данных отчета.

            //HTMLMonthLog self_obj_html_year_lcl = new(self_obj_month_logger_lcl.Sums_Sector_in!, self_obj_month_logger_lcl.Protocol_Names_Sector_in!, month);

            // Лог в HTML файл.

            IHTMLDocumentCreator.CreateLogFile(work_spaces[3].FullName,
                                               string.Join("", self_obj_html_year_lcl.Log_Data_in),
                                               Periods.MONTHES.IndexOf(month));

            // Вывод отчета в консоль.

            BackupInfo.ShowLogHeader(month);
            self_obj_log_show_in = new(self_obj_backup_per_month_lcl.Self_Obj_Sums_in.Main_Protocols_Sums_in, self_obj_backup_per_month_lcl.Self_Obj_Sums_in.Simple_Protocols_Sums_in);
            self_obj_log_show_in.ShowLog();







            // Если копировали за декабрь, то подводим итоги года, рассчетом сумм всех месяцев из их логов.

            if (month == Periods.MONTHES[Periods.DECEMBER_INDEX])
            {
                TotalLogSumsToYearCalculator self_obj_year_calc_result_lcl = new(self_obj_month_log_file_in!, self_obj_year_log_file_in!);

                Console.WriteLine('\n');
                GeneralInfo.ShowLine();
                Console.WriteLine('\n');

                // Выводим отчет за год в консоль.

                BackupInfo.ShowLogHeader(CurrentDate.Current_Year_Print_in);

                //var year_sums_lcl = self_obj_year_calc_result_lcl.GetYearSums();

                //self_obj_log_show_in.All_Protocol_Sums_in = year_sums_lcl.Item1;
                //self_obj_log_show_in.Simple_Protocol_Sums_in = year_sums_lcl.Item2;

                self_obj_log_show_in.ShowLog();
            }
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
