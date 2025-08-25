using BackupBlock;
using Logging;
using TextData;
using System.Text.RegularExpressions;


namespace Tracing
{
    abstract class BackupProcess
    {
        private readonly SourceFiles self_obj_source_files;
        private readonly DirectoryInfo backup_directory;
        private protected readonly DirectoryInfo log_directory;

        private protected string current_year_print = string.Concat(CurrentDate.Year, " ", AppConstants.year.ToLower(System.Globalization.CultureInfo.CurrentCulture));
        private protected const char slash = '\\';        // in text data
        private protected ConsoleOutFullLog? self_obj_log_show;
        private protected XYearLogFile? self_obj_year_log_file;
        private protected XMonthLogFile? self_obj_month_log_file;

        public BackupProcess(List<Drive> drives)
        {
            self_obj_source_files = new(drives[0].Directory!.FullName);

            backup_directory = CheckYearSubdirectory(drives[1].Directory!);
            log_directory = CheckYearSubdirectory(drives[2].Directory!);

            self_obj_month_log_file = new(string.Concat(log_directory.FullName, slash, AppConstants.month_logs_file));
            self_obj_year_log_file = new(string.Concat(log_directory.FullName, slash, AppConstants.year_log_file));
        }

        private protected static string CreatePeriodPattern(int month_index)
        {
            string month;
            int month_value = month_index + 1;

            if (month_index < AppConstants.october_index)
            {
                month = $"0{month_value}";
            }
            else
            {
                month = month_value.ToString();
            }

            return string.Concat(slash, "d{2}", slash, '.', month, slash, '.', CurrentDate.Year, slash, '.', AppConstants.protocol_file_type, '$');
        }

        private protected List<FileInfo>? GetEIASFiles(string period_pattern)
        {
            return self_obj_source_files.GrabMatchedFiles(new(string.Concat(AppConstants.eias_number_pattern, period_pattern), RegexOptions.IgnoreCase));
        }

        private protected Dictionary<string, List<FileInfo>>? GetSimpleFiles(string period_pattern)
        {
            Dictionary<string, List<FileInfo>> files = [];

            for (int type_index = 0; type_index < AppConstants.types_full_names.Count; type_index++)
            {
                var current_files = self_obj_source_files.GrabMatchedFiles(new(string.Concat(AppConstants.simple_number_pattern, AppConstants.types_short_names[type_index], AppConstants.line, period_pattern), RegexOptions.IgnoreCase));

                if (current_files is not null)
                {
                    files.Add(AppConstants.types_full_names[type_index], current_files);
                }
            }

            if (files.Count > 0)
            {
                return files;
            }
            else
            {
                return null;
            }
        }

        private protected static DirectoryInfo CheckYearSubdirectory(DirectoryInfo directory)
        {
            string year_backup_subdirectory = CurrentDate.Year.ToString();
                        
            DirectoryInfo destination = new(string.Concat(directory, slash, year_backup_subdirectory));

            if (!destination.Exists)
            {
                directory.CreateSubdirectory(year_backup_subdirectory);
            }
            
            return destination;
        }

        private protected int CopyBackupFiles(List<FileInfo> backup_files, string month_and_type_subdir)
        {
            int files_count = 0;                     

            for (int file_index = 0; file_index < backup_files.Count; file_index++)
            {
                backup_files[file_index].CopyTo(string.Concat(backup_directory, slash, month_and_type_subdir, slash, backup_files[file_index].Name), true);   // exception !!

                files_count++;
            }

            return files_count;
        }

        private protected int CopySimpleBlock(Dictionary<string, List<FileInfo>> files, string month)
        {
            int files_count = 0;

            foreach (var item in files)
            {
                files_count += CopyBackupFiles(item.Value, string.Concat(month, slash, item.Key));
            }

            return files_count;
        }
        
        private protected int Backuping(string target_month, List<FileInfo>? eias_files, Dictionary<string, List<FileInfo>>? simple_files, MonthBackupSums sums)
        {
            int backup_count = 0;
                       
            if (sums.All_Protocols[AppConstants.others_sums[1]] != 0)
            {
                if (CopyBackupFiles(eias_files!, string.Concat(target_month, slash, AppConstants.others_sums[1])) == sums.All_Protocols[AppConstants.others_sums[1]])
                {
                    backup_count += sums.All_Protocols[AppConstants.others_sums[1]];                    
                }
            }
            
            if (sums.All_Protocols[AppConstants.others_sums[2]] != 0)
            {
                if (CopySimpleBlock(simple_files!, target_month) == sums.All_Protocols[AppConstants.others_sums[2]])
                {
                    backup_count += sums.All_Protocols[AppConstants.others_sums[2]];                    
                }
            }

            return backup_count;  
        }
    }


    class BackupProcessMonth : BackupProcess
    {
        public BackupProcessMonth(List<Drive> drives, string month) : base(drives)
        {
            MonthBackupSums self_obj_sums;
            int month_index = AppConstants.month_names.IndexOf(month);

            var eias_files = GetEIASFiles(CreatePeriodPattern(month_index));
            var simple_files = GetSimpleFiles(CreatePeriodPattern(month_index));

            if (month_index != AppConstants.january_index)
            {
                self_obj_sums = new(eias_files, simple_files, GetSimpleFiles(CreatePeriodPattern(month_index - 1)));
            }
            else
            {
                self_obj_sums = new(eias_files, simple_files);
            }
                        
            if (self_obj_sums.All_Protocols[AppConstants.others_sums[0]] != 0)
            {
                if (Backuping(month, eias_files, simple_files, self_obj_sums) == self_obj_sums.All_Protocols[AppConstants.others_sums[0]])
                {
                    Console.WriteLine('\n');
                    AppInfoConsoleOut.ShowResult();
                    AppInfoConsoleOut.ShowStarLine();
                    Console.WriteLine('\n');
                    
                    _ = new MonthLogger(self_obj_month_log_file!, month, self_obj_sums, eias_files);

                    AppInfoConsoleOut.ShowLogHeader(month);
                    self_obj_log_show = new(self_obj_sums.All_Protocols, self_obj_sums.Simple_Protocols_Sums);
                    self_obj_log_show.ShowLog();
                                        
                    if (month == AppConstants.month_names[AppConstants.december_index])
                    {
                        YearLogResultCalculate self_obj_year_calc_result = new(self_obj_month_log_file!, self_obj_year_log_file!);

                        Console.WriteLine('\n');
                        AppInfoConsoleOut.ShowStarLine();
                        Console.WriteLine('\n');

                        AppInfoConsoleOut.ShowLogHeader(current_year_print);

                        var year_sums = self_obj_year_calc_result.GetYearSums();

                        self_obj_log_show.All_Sums = year_sums.Item1;
                        self_obj_log_show.Simple_Sums = year_sums.Item2;

                        self_obj_log_show.ShowLog();
                    }
                }
                else
                {
                    Console.WriteLine("\nCopy error");
                }
            }
            else
            {
                AppInfoConsoleOut.ShowScansNotFound(month); 
            }
        }
    }


    class BackupProcessYear : BackupProcess
    {
        private readonly List<(string, List<FileInfo>?, Dictionary<string, List<FileInfo>>?, MonthBackupSums)> year_full_backup = [];
        
        public BackupProcessYear(List<Drive> drives) : base(drives)
        {
            if (FindAllYearFiles())
            {
                // copy and logging
                var all_sums = IGeneralSums.CreateTable();
                var simple_sums = ISimpleProtocolsSums.CreateTable();

                foreach (var month_item in year_full_backup)
                {
                    foreach (var sum in month_item.Item4.All_Protocols)
                    {
                        all_sums[sum.Key] += sum.Value;
                    }

                    if (month_item.Item4.Simple_Protocols_Sums is not null)
                    {
                        foreach (var sum in month_item.Item4.Simple_Protocols_Sums)
                        {
                            simple_sums[sum.Key] += sum.Value;
                        }
                    }
                }

                if (YearBackupAndLog() == all_sums[AppConstants.others_sums[0]])
                {
                    _ = new YearLogger(self_obj_year_log_file!, all_sums, simple_sums);           // простых может и не быть

                    AppInfoConsoleOut.ShowResult();
                    AppInfoConsoleOut.ShowStarLine();

                    AppInfoConsoleOut.ShowLogHeader(current_year_print);
                    self_obj_log_show = new(all_sums, simple_sums);
                    self_obj_log_show.ShowLog();
                }
                else
                {
                    // copy error
                }
            }
            else
            {
                AppInfoConsoleOut.ShowScansNotFound(current_year_print);
            }
        }

        private bool FindAllYearFiles()
        {
            List<Dictionary<string, List<FileInfo>>?> simple_files_trace = [];

            for (int month_index = 0; month_index < AppConstants.month_names.Count; month_index++)
            {
                MonthBackupSums self_obj_sums;

                var eias_files = GetEIASFiles(CreatePeriodPattern(month_index));
                var simple_files = GetSimpleFiles(CreatePeriodPattern(month_index));

                simple_files_trace.Add(simple_files);

                if (month_index != AppConstants.january_index)
                {
                    self_obj_sums = new(eias_files, simple_files, simple_files_trace[month_index - 1]);
                }
                else
                {
                    self_obj_sums = new(eias_files, simple_files);
                }

                if (self_obj_sums.All_Protocols[AppConstants.others_sums[0]] != 0)
                {
                    year_full_backup.Add((AppConstants.month_names[month_index], eias_files, simple_files, self_obj_sums));
                }
            }

            if (year_full_backup.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int YearBackupAndLog()
        {
            int backup_count = 0;

            foreach (var month_item in year_full_backup)
            {
                if (Backuping(month_item.Item1, month_item.Item2, month_item.Item3, month_item.Item4) == month_item.Item4.All_Protocols[AppConstants.others_sums[0]])
                {
                    AppInfoConsoleOut.ShowMonthBackupResult(month_item.Item1, month_item.Item4.All_Protocols[AppConstants.others_sums[0]]);
                    AppInfoConsoleOut.ShowLine();

                    backup_count += month_item.Item4.All_Protocols[AppConstants.others_sums[0]];
                    
                    _ = new MonthLogger(self_obj_month_log_file!, month_item.Item1, month_item.Item4, month_item.Item2);
                }
            }

            return backup_count;
        }
    }
}
