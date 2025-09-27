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

            _ = new XMLMonthLogger(self_obj_month_log_file_in, month, self_obj_backup_per_month_lcl.Self_Obj_Sums_in,
                                   self_obj_backup_per_month_lcl.Self_Obj_Names_Computing_in);

            // Создание HTML данных отчета.

            HTMLMonthLogCreator self_obj_html_lcl = new(self_obj_backup_per_month_lcl.Self_Obj_Sums_in.Main_Protocols_Sums_in,
                                                             self_obj_backup_per_month_lcl.Self_Obj_Sums_in.Simple_Protocols_Sums_in,
                                                             self_obj_backup_per_month_lcl.Self_Obj_Names_Computing_in, month);

            // Лог в HTML файл.

            IHTMLDocumentCreator.CreateLogFile(work_spaces[3].FullName, string.Join("", self_obj_html_lcl.Log_Data_in), Periods.MONTHES.IndexOf(month));

            // Вывод отчета в консоль.

            BackupInfo.ShowLogHeader(month);
            self_obj_log_show_in = new(self_obj_backup_per_month_lcl.Self_Obj_Sums_in.Main_Protocols_Sums_in, 
                                       self_obj_backup_per_month_lcl.Self_Obj_Sums_in.Simple_Protocols_Sums_in);
            self_obj_log_show_in.ShowLog();

            //    * Отдельный случай ! *

            // Если копировали за декабрь, то подводим итоги года, рассчетом сумм всех месяцев и пишем логи.

            if (month == Periods.MONTHES[Periods.DECEMBER_INDEX])
            {
                // XML лог.

                TotalLogSumsToYearCalculator self_obj_year_calc_result_lcl = new(self_obj_month_log_file_in!, self_obj_year_log_file_in!);

                var year_sums_lcl = IYearSumsDataTypeConverter.GetYearSums(self_obj_year_calc_result_lcl.calculated_sums_in);

                // HTML лог.

                HTMLYearLogCreator self_obj_html_year_log_lcl = new(year_sums_lcl.Item1, year_sums_lcl.Item2);

                IHTMLDocumentCreator.CreateLogFile(work_spaces[3].FullName, string.Join("", self_obj_html_year_log_lcl.Log_Data_in), CurrentDate.Year_in);

                // Выводим отчет за год в консоль.

                Console.WriteLine('\n');
                GeneralInfo.ShowLine();
                Console.WriteLine('\n');
                                
                BackupInfo.ShowLogHeader(CurrentDate.Current_Year_Print_in);
                                
                self_obj_log_show_in.Main_Protocol_Sums_in = year_sums_lcl.Item1;
                self_obj_log_show_in.Simple_Protocol_Sums_in = year_sums_lcl.Item2;

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
