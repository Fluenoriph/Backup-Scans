/*
 * Файл "BackupProcess.cs": процесс резервного копирования.
 * 
 * 1. "BaseBackupProcess": базовый класс, для классов месячного и годового процесса копирования;
 * 2. "MonthBackupProcess": класс для резервного копирования за месяц;
 * 3. "YearBackupProcess": класс для резервного копирования за год.
 */

using InfoOut;
using ResultLogOut;
using System.Globalization;
using System.Security;
using System.Text.RegularExpressions;


abstract class BaseBackupProcess
{
    // Исходные файлы "PDF".

    readonly SourceFiles? self_obj_source_files_in;

    // Директория резервного копирования.

    readonly DirectoryInfo? backup_directory_in;

    // Директория отчетов о бэкапе.

    readonly DirectoryInfo? log_directory_in;

    // Файл отчета (лога) за год.

    protected YearLogFile? self_obj_year_log_file_in;

    // Файл отчета за месяц.

    protected MonthLogFile? self_obj_month_log_file_in;
            
    // Шаблон, для вывода отчета в консоль.

    protected FullLogPrinter? self_obj_log_show_in;

    protected string current_year_print_in = string.Concat(CurrentDate.Year, " ", PeriodsNames.YEAR.ToLower(CultureInfo.CurrentCulture));

    // Вход: список "рабочих дисков".

    public BaseBackupProcess(List<DrivesConfiguration> work_drives)
    {
        // Создание "основы" для запуска копирования.

        try
        {
            self_obj_source_files_in = new(new(work_drives[0].Work_Directory_in!));

            backup_directory_in = new(work_drives[1].Work_Directory_in!);

            // Принудительная проверка на существование папки года для резервного хранилища.

            backup_directory_in.CreateSubdirectory(CurrentDate.Year.ToString(CultureInfo.CurrentCulture));

            log_directory_in = new(work_drives[2].Work_Directory_in!);

            // Принудительная проверка на существование папки года для отчетов.

            log_directory_in.CreateSubdirectory(CurrentDate.Year.ToString(CultureInfo.CurrentCulture));
        }
        catch (IOException error)
        {
            _ = new ProgramShutDown(ErrorCode.DRIVE_RESOURCE_UNAVAILABLE, error.Message);
        }
        catch (SecurityException error)
        {
            _ = new ProgramShutDown(ErrorCode.DRIVE_RESOURCE_ACCESS_ERROR, error.Message);            
        }
                                                                            
        self_obj_month_log_file_in = new(string.Concat(log_directory_in!.FullName, Symbols.SLASH, LogFiles.MONTH_LOG_FILE));

        self_obj_year_log_file_in = new(string.Concat(log_directory_in!.FullName, Symbols.SLASH, LogFiles.YEAR_LOG_FILE));
    }

    // * Создание паттерна даты, для соединения с паттерном типа протокола. Формат: дд.мм.гг. *

    //   Параметр: индекс месяца начиная с нуля.

    protected static string CreateDatePattern(int month_index)
    {
        // Порядковое представление числа месяца.

        int month_value_lcl = month_index + 1;

        string month_lcl;

        // Создание подпаттерна месяца в зависимости от месяца бэкапа. Если нужно, то добавить символ нуля.

        if (month_index < PeriodsNames.OCTOBER_INDEX)
        {
            month_lcl = string.Concat(Symbols.NULL, month_value_lcl);
        }
        else
        {
            month_lcl = month_value_lcl.ToString(CultureInfo.CurrentCulture);
        }

        return string.Concat(Symbols.SLASH, "d{2}", Symbols.SLASH, '.', month_lcl, Symbols.SLASH, '.', CurrentDate.Year, Symbols.SLASH, '.', FilePatterns.PROTOCOL_SCAN_FILE_TYPE, '$');
    }

    // * Поиск протоколов ЕИАС. *

    protected List<FileInfo>? GetEIASFiles(string date_pattern)
    {
        return self_obj_source_files_in!.GrabMatchedFiles(new(string.Concat(FilePatterns.EIAS_NUMBER_PATTERN, date_pattern), RegexOptions.IgnoreCase));
    }

    // * Поиск простых протоколов. *

    protected Dictionary<string, List<FileInfo>>? GetSimpleFiles(string date_pattern)
    {
        Dictionary<string, List<FileInfo>> files_lcl = [];

        // Поиск по паттерну типа простого протокола и добавление в словарь по названию типов.

        for (int type_index = 0; type_index < ProtocolTypesAndSums.TYPES_FULL_NAMES.Count; type_index++)
        {
            var current_files_lcl = self_obj_source_files_in!.GrabMatchedFiles(new(string.Concat(FilePatterns.SIMPLE_NUMBER_PATTERN, ProtocolTypesAndSums.TYPES_SHORT_NAMES[type_index], Symbols.LINE, date_pattern), RegexOptions.IgnoreCase));

            if (current_files_lcl is not null)
            {
                files_lcl.Add(ProtocolTypesAndSums.TYPES_FULL_NAMES[type_index], current_files_lcl);
            }
        }

        if (files_lcl.Count != 0)
        {
            return files_lcl;
        }
        else
        {
            return null;
        }
    }

    // * Копирование списка файлов. *

    //   Параметры: "backup_files" - файлы, "month_and_type_subdir" - поддиректория .\Месяц\Тип протоколов.

    protected int CopyBackupFiles(List<FileInfo> backup_files, string month_and_type_subdir)
    {
        // Счетчик скопированных файлов.

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
                _ = new ProgramCrash(ErrorCode.COPY_FILE_ERROR, error.Message);                
            }           
        }

        return backuping_files_count_lcl;
    }

    // * Копирование протоколов "физ. факторы". *
        
    protected int CopySimpleBlock(Dictionary<string, List<FileInfo>> files, string month)
    {
        // Счетчик.

        int backuping_files_count_lcl = 0;

        foreach (var item in files)
        {
            // Копирование в поддиректорию .\"month"\"item.Key".

            backuping_files_count_lcl += CopyBackupFiles(item.Value, string.Concat(month, Symbols.SLASH, item.Key));
        }

        return backuping_files_count_lcl;
    }

    // * Копирование за месяц. *

    // Параметры: месяц, сканы ЕИАС, сканы по "ФФ", суммы бэкапа за данный месяц.

    protected int MonthBackuping(string current_month, List<FileInfo>? eias_files, Dictionary<string, List<FileInfo>>? simple_files, MonthBackupSums sums)
    {
        // Счетчик всех скопированных файлов за месяц.

        int backup_count_lcl = 0;

        // Копирование, при условии, что есть ЕИАС сканы в этом месяце.

        if (sums.All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[1]] != 0)
        {
            // Контроль сумм, найденных и скопированных.

            if (CopyBackupFiles(eias_files!, string.Concat(current_month, Symbols.SLASH, ProtocolTypesAndSums.OTHERS_SUMS[1])) == sums.All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[1]])
            {
                backup_count_lcl += sums.All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[1]];
            }
        }

        // Копирование, при условии, что есть "ФФ" сканы в этом месяце.

        if (sums.All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[2]] != 0)
        {
            // Контроль сумм.

            if (CopySimpleBlock(simple_files!, current_month) == sums.All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[2]])
            {
                backup_count_lcl += sums.All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[2]];
            }
        }

        return backup_count_lcl;
    }
}


class MonthBackupProcess : BaseBackupProcess
{
    // Параметр: "month" - месяц, за который выполняется копирование.

    public MonthBackupProcess(List<DrivesConfiguration> work_drives, string month) : base(work_drives)
    {
        MonthBackupSums sums_lcl;

        // Создание индекса месяца и поиск протоколов.

        int month_index_lcl = PeriodsNames.MONTHES.IndexOf(month);

        var eias_files_lcl = GetEIASFiles(CreateDatePattern(month_index_lcl));
        var simple_files_lcl = GetSimpleFiles(CreateDatePattern(month_index_lcl));

        // Если производится копирование за любой месяц, кроме января, то вычисляются неизвестные протоколы.

        if (month_index_lcl != PeriodsNames.JANUARY_INDEX)
        {
            sums_lcl = new(eias_files_lcl, simple_files_lcl, GetSimpleFiles(CreateDatePattern(month_index_lcl - 1)));
        }
        else
        {
            sums_lcl = new(eias_files_lcl, simple_files_lcl);
        }

        // Если в текущем месяце есть протоколы, то далее ...

        if (sums_lcl.All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[0]] != 0)
        {
            BackupInfo.ShowVisualWait();

            // Контроль скопированной суммы. При отрицательном результате, генерируется ошибка.

            if (MonthBackuping(month, eias_files_lcl, simple_files_lcl, sums_lcl) == sums_lcl.All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[0]])
            {
                // Логгинг отчета.

                _ = new MonthLogger(self_obj_month_log_file_in!, month, sums_lcl, eias_files_lcl);

                Console.Clear();
                                
                BackupInfo.ShowResult();
                GeneralInfo.ShowStarLine();
                Console.WriteLine('\n');
                                
                // Вывод отчета в консоль.

                BackupInfo.ShowLogHeader(month);
                self_obj_log_show_in = new(sums_lcl.All_Protocols_Sums_in, sums_lcl.Simple_Protocols_Sums_in);
                self_obj_log_show_in.ShowLog();

                // Если копировали за декабрь, то подводим итоги года, рассчетом сумм всех месяцев из их логов.

                if (month == PeriodsNames.MONTHES[PeriodsNames.DECEMBER_INDEX])
                {
                    TotalLogSumsToYearCalculator year_calc_result_lcl = new(self_obj_month_log_file_in!, self_obj_year_log_file_in!);
                                        
                    Console.WriteLine('\n');
                    GeneralInfo.ShowLine();
                    Console.WriteLine('\n');

                    BackupInfo.ShowLogHeader(current_year_print_in);

                    var year_sums_lcl = year_calc_result_lcl.GetYearSums();

                    // Выводим отчет за год в консоль.

                    self_obj_log_show_in.All_Protocol_Sums_in = year_sums_lcl.Item1;
                    self_obj_log_show_in.Simple_Protocol_Sums_in = year_sums_lcl.Item2;

                    self_obj_log_show_in.ShowLog();
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


class YearBackupProcess : BaseBackupProcess
{
    // Список по валидным месяцам всех данных бэкапа за год.

    readonly List<(string, List<FileInfo>?, Dictionary<string, List<FileInfo>>?, MonthBackupSums)> year_full_backup_in = [];

    public YearBackupProcess(List<DrivesConfiguration> work_drives) : base(work_drives)
    {
        // Продолжение процесса, если хотя бы за один месяц есть файлы.

        if (FindAllYearFiles())
        {
            // Сложение всех сумм за год, для оперативного формирования отчета.

            var all_sums_lcl = ISumsTableCreator.Create(ProtocolTypesAndSums.OTHERS_SUMS);
            var simple_sums_lcl = ISumsTableCreator.Create(ProtocolTypesAndSums.UNITED_SIMPLE_TYPE_SUMS);

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

            // Если контроль сумм прошел успешно, то продолжаем далее, если нет, то генерация ошибки.

            if (YearBackupingAndLogging() == all_sums_lcl[ProtocolTypesAndSums.OTHERS_SUMS[0]])
            {
                // Логгинг.

                _ = new YearLogger(self_obj_year_log_file_in!, all_sums_lcl, simple_sums_lcl);

                Console.Clear();

                BackupInfo.ShowResult();
                GeneralInfo.ShowStarLine();

                // Вывод на консоль количества скопированных файлов за каждый месяц.

                foreach (var month_item in year_full_backup_in)
                {
                    BackupInfo.ShowMonthBackupResult(month_item.Item1, month_item.Item4.All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[0]]);
                    GeneralInfo.ShowLine();
                }

                Console.WriteLine('\n');
                
                // Вывод отчета за год.

                BackupInfo.ShowLogHeader(current_year_print_in);
                self_obj_log_show_in = new(all_sums_lcl, simple_sums_lcl);
                self_obj_log_show_in.ShowLog();
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

    // * Поиск протоколов по всем месяцам. *

    bool FindAllYearFiles()
    {
        // Список простых протоколов ("ФФ"), предназначенный для динамического поиска неизвестных протоколов.

        List<Dictionary<string, List<FileInfo>>?> simple_files_trace_lcl = [];

        for (int month_index = 0; month_index < PeriodsNames.MONTHES.Count; month_index++)
        {
            MonthBackupSums sums_in;

            var eias_files_lcl = GetEIASFiles(CreateDatePattern(month_index));
            var simple_files_lcl = GetSimpleFiles(CreateDatePattern(month_index));

            // Добавление коллекции простых протоколов. Если NULL, то это значит что в данном месяце нет этих файлов.

            simple_files_trace_lcl.Add(simple_files_lcl);

            // За текущий месяц, кроме января, вычисляем неизвестные протоколы. 

            if (month_index != PeriodsNames.JANUARY_INDEX)
            {
                // Следовательно, если коллекция простых протоколов "simple_files_trace_lcl[month_index - 1]" равна NULL, то неизвестные не вычисляются.

                sums_in = new(eias_files_lcl, simple_files_lcl, simple_files_trace_lcl[month_index - 1]);
            }
            else
            {
                sums_in = new(eias_files_lcl, simple_files_lcl);
            }

            // Добавление, только если есть файлы в текущем месяце.

            if (sums_in.All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[0]] != 0)
            {
                year_full_backup_in.Add((PeriodsNames.MONTHES[month_index], eias_files_lcl, simple_files_lcl, sums_in));
            }
        }

        // Проверка поиска.

        if (year_full_backup_in.Count != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // * Бэкап и лог. *

    int YearBackupingAndLogging()
    {
        // Счетчик.

        int backup_count_lcl = 0;

        foreach (var month_item in year_full_backup_in)
        {
            // Контроль сумм.

            if (MonthBackuping(month_item.Item1, month_item.Item2, month_item.Item3, month_item.Item4) == month_item.Item4.All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[0]])
            {
                backup_count_lcl += month_item.Item4.All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[0]];

                _ = new MonthLogger(self_obj_month_log_file_in!, month_item.Item1, month_item.Item4, month_item.Item2);
            }
        }

        return backup_count_lcl;
    }
}
