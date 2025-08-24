using BackupBlock;
using Logging;
using TextData;
using System.Text.RegularExpressions;


namespace Tracing
{
    abstract class BackupProcess(List<Drive> drives)
    {
        private readonly SourceFiles self_obj_source_files = new(drives[0].Directory!.FullName);
        private readonly DirectoryInfo backup_directory = CheckYearSubdirectory(drives[1].Directory!);
        private protected readonly DirectoryInfo log_directory = CheckYearSubdirectory(drives[2].Directory!);

        private protected string Period { get; set; } = string.Empty;
        private protected const char slash = '\\';        // in text data
        private protected ConsoleOutFullLog? log_show;
                      
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
            Period = month;
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
                if (Backuping(Period, eias_files, simple_files, self_obj_sums) == self_obj_sums.All_Protocols[AppConstants.others_sums[0]])
                {
                    Console.WriteLine('\n');
                    AppInfoConsoleOut.ShowResult();
                    AppInfoConsoleOut.ShowStarLine();
                    Console.WriteLine('\n');
                    // var log file
                    _ = new MonthLogger(log_directory, Period, self_obj_sums, eias_files);

                    AppInfoConsoleOut.ShowLogHeader(Period);
                    log_show = new(self_obj_sums.All_Protocols, self_obj_sums.Simple_Protocols_Sums);
                    log_show.ShowLog();
                }
                else
                {
                    Console.WriteLine("\nCopy error");
                }
            }
            else
            {
                AppInfoConsoleOut.ShowScansNotFound(Period); 
            }
        }
    }


    class BackupProcessYear : BackupProcess
    {
        private readonly List<(string, List<FileInfo>?, Dictionary<string, List<FileInfo>>?, MonthBackupSums)> year_full_backup = [];
        
        public BackupProcessYear(List<Drive> drives) : base(drives)
        {
            Period = string.Concat(CurrentDate.Year, AppConstants.year);

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
                    _ = new YearLogger(log_directory, all_sums, simple_sums);           // простых может и не быть

                    AppInfoConsoleOut.ShowResult();
                    AppInfoConsoleOut.ShowStarLine();

                    AppInfoConsoleOut.ShowLogHeader(Period);
                    log_show = new(all_sums, simple_sums);
                    log_show.ShowLog();
                }
                else
                {
                    // copy error
                }
            }
            else
            {
                AppInfoConsoleOut.ShowScansNotFound(Period);
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
                    // каждый раз проверка файла лога ??
                    _ = new MonthLogger(log_directory, month_item.Item1, month_item.Item4, month_item.Item2);
                }
            }

            return backup_count;
        }
    }
}
