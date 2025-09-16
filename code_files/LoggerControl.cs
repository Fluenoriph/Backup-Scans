using ResultLogOut;


abstract class BaseLoggerControl
{


    FullLogPrinter self_obj_log_show_in;



}




/*// Логгинг отчета.

                _ = new MonthLogger(self_obj_month_log_file_in!, month, sums_lcl, eias_files_lcl);

                Console.Clear();
                                
                BackupInfo.ShowResult();
                GeneralInfo.ShowStarLine();
                Console.WriteLine('\n');
                                







                // Класс создатель отчетов высокоуровневый !!!!!

                // ******************************************
                // Вывод отчета в консоль.

                BackupInfo.ShowLogHeader(month);
                self_obj_log_show_in = new(sums_lcl.All_Protocols_Sums_in, sums_lcl.Simple_Protocols_Sums_in);
                self_obj_log_show_in.ShowLog();

                // Если копировали за декабрь, то подводим итоги года, рассчетом сумм всех месяцев из их логов.

                // Также отдельный класс.....  


                if (month == PeriodsNames.MONTHES[PeriodsNames.DECEMBER_INDEX])
                {
                    // Рассчет по горизонтали.... по тэгам

                    // У парсера вертикальная логика.....  другая задача

                    TotalLogSumsToYearCalculator year_calc_result_lcl = new(self_obj_month_log_file_in!, self_obj_year_log_file_in!);
                                        
                    Console.WriteLine('\n');
                    GeneralInfo.ShowLine();
                    Console.WriteLine('\n');

                    BackupInfo.ShowLogHeader(CurrentDate.Current_Year_Print_in);

                    var year_sums_lcl = year_calc_result_lcl.GetYearSums();

                    // Выводим отчет за год в консоль.

                    self_obj_log_show_in.All_Protocol_Sums_in = year_sums_lcl.Item1;
                    self_obj_log_show_in.Simple_Protocol_Sums_in = year_sums_lcl.Item2;

                    self_obj_log_show_in.ShowLog();*/


/*// Логгинг.

                // Класс создатель html --- или парсер xml или парсер словаря сумм !!!!!!!!
                
                // Парсер xml 

                _ = new YearLogger(self_obj_year_log_file_in!, all_sums_lcl, simple_sums_lcl);

                Console.Clear();

                BackupInfo.ShowResult();
                GeneralInfo.ShowStarLine();

                // Вывод на консоль количества скопированных файлов за каждый месяц.

                foreach (var month_item in year_full_backup_in)
                {
                    BackupInfo.ShowMonthBackupResult(month_item.Item1, month_item.Item4.All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[0]]);
                    GeneralInfo.ShowLine();
                }

                Console.WriteLine('\n');
                
                // Вывод отчета за год.

                BackupInfo.ShowLogHeader(CurrentDate.Current_Year_Print_in);
                self_obj_log_show_in = new(all_sums_lcl, simple_sums_lcl);
                self_obj_log_show_in.ShowLog();*/