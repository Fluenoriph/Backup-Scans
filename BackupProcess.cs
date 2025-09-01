using InfoOut;
using ResultLogOut;
using System.Globalization;
using System.Text.RegularExpressions;


abstract class BackupProcess
{
    private readonly SourceFiles? source_files_in;

    private readonly DirectoryInfo? backup_directory_in;
    private protected readonly DirectoryInfo log_directory_in;

    private protected string current_year_print_in = string.Concat(CurrentDate.Year, " ", PeriodsNames.YEAR.ToLower(CultureInfo.CurrentCulture));
    private protected FullLogPrinter? log_show_in;

    private protected YearLogFile? year_log_file_in;
    private protected MonthLogFile? month_log_file_in;

    public BackupProcess(List<DriveControl> work_drives)
    {
        try
        {
            source_files_in = new(work_drives[0].Work_Directory_in!);

            backup_directory_in = work_drives[1].Work_Directory_in!.CreateSubdirectory(CurrentDate.Year.ToString(CultureInfo.CurrentCulture));  

            log_directory_in = work_drives[2].Work_Directory_in!.CreateSubdirectory(CurrentDate.Year.ToString(CultureInfo.CurrentCulture));
        }
        catch (IOException error)
        {
            _ = new ProgramShutDown(ErrorCodes.DRIVE_RESOURCE_UNAVAILABLE, error.Message);
        }
                                                                            
        month_log_file_in = new(string.Concat(log_directory_in!.FullName, Symbols.SLASH, LogFiles.MONTH_LOG_FILE));

        year_log_file_in = new(string.Concat(log_directory_in.FullName, Symbols.SLASH, LogFiles.YEAR_LOG_FILE));
    }

    private protected static string CreatePeriodPattern(int month_index)
    {
        string month_lcl;
        int month_value_lcl = month_index + 1;

        if (month_index < PeriodsNames.OCTOBER_INDEX)
        {
            month_lcl = string.Concat(Symbols.NULL, month_value_lcl);
        }
        else
        {
            month_lcl = month_value_lcl.ToString(CultureInfo.CurrentCulture);
        }

        return string.Concat(Symbols.SLASH, "d{2}", Symbols.SLASH, '.', month_lcl, Symbols.SLASH, '.', CurrentDate.Year, Symbols.SLASH, '.', FilePatterns.PROTOCOL_SCAN_TYPE, '$');
    }

    private protected List<FileInfo>? GetEIASFiles(string period_pattern)
    {
        return source_files_in!.GrabMatchedFiles(new(string.Concat(FilePatterns.EIAS_NUMBER_PATTERN, period_pattern), RegexOptions.IgnoreCase));
    }

    private protected Dictionary<string, List<FileInfo>>? GetSimpleFiles(string period_pattern)
    {
        Dictionary<string, List<FileInfo>> files_lcl = [];

        for (int type_index = 0; type_index < ProtocolTypesSums.TYPES_FULL_NAMES.Count; type_index++)
        {
            var current_files_lcl = source_files_in!.GrabMatchedFiles(new(string.Concat(FilePatterns.SIMPLE_NUMBER_PATTERN, ProtocolTypesSums.TYPES_SHORT_NAMES[type_index], Symbols.LINE, period_pattern), RegexOptions.IgnoreCase));

            if (current_files_lcl is not null)
            {
                files_lcl.Add(ProtocolTypesSums.TYPES_FULL_NAMES[type_index], current_files_lcl);
            }
        }

        if (files_lcl.Count > 0)
        {
            return files_lcl;
        }
        else
        {
            return null;
        }
    }

    private protected int CopyBackupFiles(List<FileInfo> backup_files, string month_and_type_subdir)
    {
        int backuping_files_count_lcl = 0;
                
        for (int file_index = 0; file_index < backup_files.Count; file_index++)
        {
            try
            {
                backup_files[file_index].CopyTo(string.Concat(backup_directory_in!.CreateSubdirectory(month_and_type_subdir), Symbols.SLASH, backup_files[file_index].Name), true);
                
                backuping_files_count_lcl++;
            }
            catch (IOException error)
            {
                _ = new ProgramCrash(ErrorCodes.COPY_FILES_ERROR, error.Message);                
            }           
        }

        return backuping_files_count_lcl;
    }

    private protected int CopySimpleBlock(Dictionary<string, List<FileInfo>> files, string month)
    {
        int backuping_files_count_lcl = 0;

        foreach (var item in files)
        {
            backuping_files_count_lcl += CopyBackupFiles(item.Value, string.Concat(month, Symbols.SLASH, item.Key));
        }

        return backuping_files_count_lcl;
    }

    private protected int MonthBackuping(string target_month, List<FileInfo>? eias_files, Dictionary<string, List<FileInfo>>? simple_files, MonthBackupSums sums)
    {
        int backup_count_lcl = 0;

        if (sums.All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[1]] != 0)
        {
            if (CopyBackupFiles(eias_files!, string.Concat(target_month, Symbols.SLASH, ProtocolTypesSums.OTHERS_SUMS[1])) == sums.All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[1]])
            {
                backup_count_lcl += sums.All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[1]];
            }
        }

        if (sums.All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[2]] != 0)
        {
            if (CopySimpleBlock(simple_files!, target_month) == sums.All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[2]])
            {
                backup_count_lcl += sums.All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[2]];
            }
        }

        return backup_count_lcl;
    }
}


class BackupProcessMonth : BackupProcess
{
    public BackupProcessMonth(List<DriveControl> work_drives, string month) : base(work_drives)
    {
        MonthBackupSums sums_lcl;
        int month_index_lcl = PeriodsNames.MONTHES.IndexOf(month);

        var eias_files_lcl = GetEIASFiles(CreatePeriodPattern(month_index_lcl));
        var simple_files_lcl = GetSimpleFiles(CreatePeriodPattern(month_index_lcl));

        if (month_index_lcl != PeriodsNames.JANUARY_INDEX)
        {
            sums_lcl = new(eias_files_lcl, simple_files_lcl, GetSimpleFiles(CreatePeriodPattern(month_index_lcl - 1)));
        }
        else
        {
            sums_lcl = new(eias_files_lcl, simple_files_lcl);
        }

        if (sums_lcl.All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[0]] != 0)
        {
            BackupInfo.ShowVisualWait();

            if (MonthBackuping(month, eias_files_lcl, simple_files_lcl, sums_lcl) == sums_lcl.All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[0]])
            {
                _ = new MonthLogger(month_log_file_in!, month, sums_lcl, eias_files_lcl);

                Console.Clear();
                                
                BackupInfo.ShowResult();
                GeneralInfo.ShowStarLine();
                Console.WriteLine('\n');
                                
                BackupInfo.ShowLogHeader(month);
                log_show_in = new(sums_lcl.All_Protocols_Sums_in, sums_lcl.Simple_Protocols_Sums_in);
                log_show_in.ShowLog();

                if (month == PeriodsNames.MONTHES[PeriodsNames.DECEMBER_INDEX])
                {
                    CalculateTotalLogSumsToYear year_calc_result_lcl = new(month_log_file_in!, year_log_file_in!);
                                        
                    Console.WriteLine('\n');
                    GeneralInfo.ShowLine();
                    Console.WriteLine('\n');

                    BackupInfo.ShowLogHeader(current_year_print_in);

                    var year_sums_lcl = year_calc_result_lcl.GetYearSums();

                    log_show_in.All_Protocol_Sums_in = year_sums_lcl.Item1;
                    log_show_in.Simple_Protocol_Sums_in = year_sums_lcl.Item2;

                    log_show_in.ShowLog();
                }
            }
            else
            {
                BackupInfo.ShowCopyError();
            }
        }
        else
        {
            BackupInfo.ShowScansNotFound(month);
        }
    }
}


class BackupProcessYear : BackupProcess
{
    private readonly List<(string, List<FileInfo>?, Dictionary<string, List<FileInfo>>?, MonthBackupSums)> year_full_backup_in = [];

    public BackupProcessYear(List<DriveControl> work_drives) : base(work_drives)
    {
        if (FindAllYearFiles())
        {
            var all_sums_lcl = ISumsTableCreator.Create(ProtocolTypesSums.OTHERS_SUMS);
            var simple_sums_lcl = ISumsTableCreator.Create(ProtocolTypesSums.UNITED_SIMPLE_TYPE_SUMS);

            foreach (var month_item in year_full_backup_in)
            {
                foreach (var sum in month_item.Item4.All_Protocols_Sums_in)
                {
                    all_sums_lcl[sum.Key] += sum.Value;
                }

                if (month_item.Item4.Simple_Protocols_Sums_in is not null)
                {
                    foreach (var sum in month_item.Item4.Simple_Protocols_Sums_in)
                    {
                        simple_sums_lcl[sum.Key] += sum.Value;
                    }
                }
            }

            BackupInfo.ShowVisualWait();

            if (YearBackupingAndLogging() == all_sums_lcl[ProtocolTypesSums.OTHERS_SUMS[0]])
            {
                _ = new YearLogger(year_log_file_in!, all_sums_lcl, simple_sums_lcl);

                Console.Clear();

                BackupInfo.ShowResult();
                GeneralInfo.ShowStarLine();

                foreach (var month_item in year_full_backup_in)
                {
                    BackupInfo.ShowMonthBackupResult(month_item.Item1, month_item.Item4.All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[0]]);
                    GeneralInfo.ShowLine();
                }

                Console.WriteLine('\n');
                
                BackupInfo.ShowLogHeader(current_year_print_in);
                log_show_in = new(all_sums_lcl, simple_sums_lcl);
                log_show_in.ShowLog();
            }
            else
            {
                BackupInfo.ShowCopyError();
            }
        }
        else
        {
            BackupInfo.ShowScansNotFound(current_year_print_in);
        }
    }

    private bool FindAllYearFiles()
    {
        List<Dictionary<string, List<FileInfo>>?> simple_files_trace_lcl = [];

        for (int month_index = 0; month_index < PeriodsNames.MONTHES.Count; month_index++)
        {
            MonthBackupSums sums_in;

            var eias_files_lcl = GetEIASFiles(CreatePeriodPattern(month_index));
            var simple_files_lcl = GetSimpleFiles(CreatePeriodPattern(month_index));

            simple_files_trace_lcl.Add(simple_files_lcl);

            if (month_index != PeriodsNames.JANUARY_INDEX)
            {
                sums_in = new(eias_files_lcl, simple_files_lcl, simple_files_trace_lcl[month_index - 1]);
            }
            else
            {
                sums_in = new(eias_files_lcl, simple_files_lcl);
            }

            if (sums_in.All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[0]] != 0)
            {
                year_full_backup_in.Add((PeriodsNames.MONTHES[month_index], eias_files_lcl, simple_files_lcl, sums_in));
            }
        }

        if (year_full_backup_in.Count != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int YearBackupingAndLogging()
    {
        int backup_count_lcl = 0;

        foreach (var month_item in year_full_backup_in)
        {
            if (MonthBackuping(month_item.Item1, month_item.Item2, month_item.Item3, month_item.Item4) == month_item.Item4.All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[0]])
            {
                backup_count_lcl += month_item.Item4.All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[0]];

                _ = new MonthLogger(month_log_file_in!, month_item.Item1, month_item.Item4, month_item.Item2);
            }
        }

        return backup_count_lcl;
    }
}
