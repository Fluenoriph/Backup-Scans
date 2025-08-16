using BackupBlock;
using DrivesControl;
using Logging;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TextData;
using Tracing;


//Console.WriteLine($"{line}\n* Backup PDF v.1.0 *\n{line}\n");

XMLConfig self_obj_drives_config = new();

Console.WriteLine("\nВведите номер месяца, за который выполнить копирование:");   
// должна быть проверка правильности ввода месяца
var period = Console.ReadLine();
int period_value = Convert.ToInt32(period);
                                
if (period_value >= 1 && period_value <= 12)
{
    BackupProcessMonth _ = new(self_obj_drives_config.Drives, period_value); 
}
else if (period_value == 0)
{
    BackupProcessYear _ = new(self_obj_drives_config.Drives);
}
else
{
    Console.WriteLine("\nОшибка ввода периода !");
    System.Environment.Exit(0);
}
             

// any file
abstract class BackupProcess(List<Drive> drives)
{
    private readonly SourceFiles self_obj_source_files = new(drives[0].Directory.FullName);
    private protected const char slash = '\\';
    private protected ConsoleLogOut? log_show;

    private protected readonly Action<string> ScansNotFound = (period) => Console.WriteLine($"\nЗа {period} сканов не найдено !");

    private protected static string CreatePeriodPattern(int month_value)
    {
        string month;

        if (month_value < 10)
        {
            month = $"0{month_value}";
        }
        else
        {
            month = month_value.ToString();
        }

        return string.Concat(slash, "d{2}", slash, '.', month, slash, '.', CurrentYear.Year, slash, '.', AppConstants.scan_file_type, '$');
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
            var current_files = self_obj_source_files.GrabMatchedFiles(new(string.Concat($"{AppConstants.simple_number_pattern}{AppConstants.types_short_names[type_index]}-", period_pattern), RegexOptions.IgnoreCase));

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

    private protected int CopyBackupFiles(List<FileInfo> backup_files, string target_directory)
    {
        int files_count = 0;

        DirectoryInfo destination = new(string.Concat(drives[1].Directory.FullName, slash, target_directory)); 

        if (!destination.Exists)
        {
            drives[1].Directory.CreateSubdirectory(target_directory);
        }

        for (int file_index = 0; file_index < backup_files.Count; file_index++)        
        {
            backup_files[file_index].CopyTo(string.Concat(destination, slash, backup_files[file_index].Name), true);   // exception !!

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

    private protected int BackupAndLog(int month_value, List<FileInfo>? eias_files, Dictionary<string, List<FileInfo>>? simple_files, MonthSums sums)
    {
        int backup_count = 0;
        string target_month = AppConstants.month_names[month_value - 1];

        if (sums.All_Protocols[AppConstants.others_sums[1]] != 0)
        {
            if (CopyBackupFiles(eias_files!, string.Concat(target_month, slash, AppConstants.others_sums[1])) == sums.All_Protocols[AppConstants.others_sums[1]])
            {
                backup_count += sums.All_Protocols[AppConstants.others_sums[1]];

                _ = new EIASLog(month_value, eias_files!, sums);
            }
        }

        if (sums.All_Protocols[AppConstants.others_sums[2]] != 0)
        {
            if (CopySimpleBlock(simple_files!, target_month) == sums.All_Protocols[AppConstants.others_sums[2]])
            {
                backup_count += sums.All_Protocols[AppConstants.others_sums[2]];

                _ = new SimpleLog(month_value, sums);
            }
        }

        return backup_count;
    }
}

   
class BackupProcessMonth : BackupProcess
{
    public BackupProcessMonth(List<Drive> drives, int month_value) : base(drives)
    {
        MonthSums self_obj_sums;
        var eias_files = GetEIASFiles(CreatePeriodPattern(month_value));
        var simple_files = GetSimpleFiles(CreatePeriodPattern(month_value));
                    
        if (month_value != 1)
        {
            self_obj_sums = new(eias_files, simple_files, GetSimpleFiles(CreatePeriodPattern(month_value - 1)));                
        }
        else
        {
            self_obj_sums = new(eias_files, simple_files);                
        }               

        if (self_obj_sums.All_Protocols[AppConstants.others_sums[0]] != 0)
        {
            if (BackupAndLog(month_value, eias_files, simple_files, self_obj_sums) == self_obj_sums.All_Protocols[AppConstants.others_sums[0]])
            {
                Console.WriteLine($"\nУспех ! За {AppConstants.month_names[month_value - 1]} скопировано {self_obj_sums.All_Protocols[AppConstants.others_sums[0]]} файлов !\n");

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
            ScansNotFound(AppConstants.month_names[month_value - 1]);
        }
    }
}


class BackupProcessYear : BackupProcess
{
    private readonly List<(int, List<FileInfo>?, Dictionary<string, List<FileInfo>>?, MonthSums)> year_full_backup = [];
    
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
                // ok!  log year data !!
                _ = new YearLog(all_sums, simple_sums);
                Console.WriteLine($" *** Полный отчет за {CurrentYear.Year} год ***");
                ConsoleLogOut log_show = new(all_sums, simple_sums);
                log_show.ShowLog();
            }
            else
            {
                // copy error
            }
        }
        else
        {
            ScansNotFound(string.Concat(CurrentYear.Year, AppConstants.year));
        }
    }

    private bool FindAllYearFiles()
    {
        List<Dictionary<string, List<FileInfo>>?> simple_files_trace = [];

        for (int month_index = 0; month_index < AppConstants.month_names.Count; month_index++)
        {
            MonthSums self_obj_sums;
            var eias_files = GetEIASFiles(CreatePeriodPattern(month_index + 1));
            var simple_files = GetSimpleFiles(CreatePeriodPattern(month_index + 1));

            simple_files_trace.Add(simple_files);

            if (month_index > 0)
            {
                self_obj_sums = new(eias_files, simple_files, simple_files_trace[month_index - 1]);
            }
            else
            {
                self_obj_sums = new(eias_files, simple_files);
            }

            if (self_obj_sums.All_Protocols[AppConstants.others_sums[0]] != 0)
            {
                year_full_backup.Add((month_index + 1, eias_files, simple_files, self_obj_sums)); 
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
            if (BackupAndLog(month_item.Item1, month_item.Item2, month_item.Item3, month_item.Item4) == month_item.Item4.All_Protocols[AppConstants.others_sums[0]])
            {
                Console.WriteLine($"\nУспех ! За {AppConstants.month_names[month_item.Item1 - 1]} скопировано {month_item.Item4.All_Protocols[AppConstants.others_sums[0]]} файлов !\n");
                backup_count += month_item.Item4.All_Protocols[AppConstants.others_sums[0]];
            }
        }

        return backup_count;
    }
}



