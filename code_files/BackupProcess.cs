/*
 * Файл "BackupProcess.cs": процесс резервного копирования.
 * 
 * 1. "BaseBackupProcess": базовый класс, для классов месячного и годового процесса копирования;
 * 2. "MonthBackupProcess": класс для резервного копирования за месяц;
 * 3. "YearBackupProcess": класс для резервного копирования за год.
 */

using InfoOut;
using System.Globalization;
using System.Text.RegularExpressions;


enum BackupingStatusCode
{
    BACKUP_FAILURE = 0,
    BACKUP_SUCCESS = 1,
    BACKUP_NOT_FOUND = 2
}


// * Параметры: исходная директория, резервная директория. *

abstract class BaseBackupProcess(DirectoryInfo source_directory, DirectoryInfo backup_directory)
{
    // Исходные файлы "PDF".

    readonly SourceFiles? self_obj_source_files_in = new(source_directory);

    public BackupingStatusCode Backup_status_in { get; set; }

    // * Создание паттерна даты, для соединения с паттерном типа протокола. Формат: дд.мм.гг. *
    //   Параметр: индекс месяца начиная с нуля.

    protected static string CreateDatePattern(int month_index)
    {
        const char SLASH = '\\';
        const char POINT = '.';

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

        return string.Concat(SLASH, "d{2}", SLASH, POINT, month_lcl, SLASH, POINT, CurrentDate.Year_in, SLASH, POINT, FilePatterns.PROTOCOL_SCAN_FILE_TYPE, '$');
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
                backup_files[file_index].CopyTo(Path.Combine(backup_directory.CreateSubdirectory(month_and_type_subdir).FullName, backup_files[file_index].Name), true);
                
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

            backuping_files_count_lcl += CopyBackupFiles(item.Value, Path.Join(month, item.Key));
        }

        return backuping_files_count_lcl;
    }

    // * Копирование за месяц. *

    // Параметры: месяц, сканы ЕИАС, сканы по "ФФ", суммы бэкапа за данный месяц.

    // Здесь нужны только общие суммы для контроля условий копирования.
    
    protected int MonthBackuping(string current_month, List<FileInfo>? eias_files, Dictionary<string, List<FileInfo>>? simple_files, BackupSumsPerMonth sums)
    {
        // Счетчик всех скопированных файлов за месяц.

        int backup_count_lcl = 0;

        // Копирование, при условии, что есть ЕИАС сканы в этом месяце.

        if (sums.All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[1]] != 0)
        {
            // Контроль сумм, найденных и скопированных.

            if (CopyBackupFiles(eias_files!, Path.Join(current_month, ProtocolTypesAndSums.MAIN_SUMS[1])) == sums.All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[1]])
            {
                backup_count_lcl += sums.All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[1]];
            }
        }

        // Копирование, при условии, что есть "ФФ" сканы в этом месяце.

        if (sums.All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[2]] != 0)
        {
            // Контроль сумм.

            if (CopySimpleBlock(simple_files!, current_month) == sums.All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[2]])
            {
                backup_count_lcl += sums.All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[2]];
            }
        }

        return backup_count_lcl;
    }
}


class MonthBackupProcess : BaseBackupProcess
{
    public ProtocolNamesComputingPerMonth Self_Obj_Names_Computing_in { get; }
    public BackupSumsPerMonth Self_Obj_Sums_in { get; }
    
    // Параметр: "month" - месяц, за который выполняется копирование.

    public MonthBackupProcess(DirectoryInfo source_directory, DirectoryInfo backup_directory, string month) : base(source_directory, backup_directory)
    {
        // Создание индекса месяца и поиск протоколов.

        int month_index_lcl = PeriodsNames.MONTHES.IndexOf(month);

        var eias_files_lcl = GetEIASFiles(CreateDatePattern(month_index_lcl));
        var simple_files_lcl = GetSimpleFiles(CreateDatePattern(month_index_lcl));

        Self_Obj_Names_Computing_in = new(eias_files_lcl, simple_files_lcl);

        // Если производится копирование за любой месяц, кроме января, то вычисляются неизвестные протоколы.

        if (month_index_lcl != PeriodsNames.JANUARY_INDEX)
        {
            // Согласно алгоритму, нужно получить номера протоколов предыдущего периода.
                        
            Self_Obj_Names_Computing_in.ComputeUnknownProtocols(GetSimpleFiles(CreateDatePattern(month_index_lcl - 1)));
        }

        // Рассчитываем суммы бэкапа.

        Self_Obj_Sums_in = new(Self_Obj_Names_Computing_in.Sorted_Eias_Protocol_Names_in,
                               Self_Obj_Names_Computing_in.Sorted_Simple_Protocol_Names_in,
                               Self_Obj_Names_Computing_in.Missed_Simple_Protocols_in,
                               Self_Obj_Names_Computing_in.Unknown_Simple_Protocols_in);

        // Если в текущем месяце есть протоколы, то копируем.

        if (Self_Obj_Sums_in.All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[0]] != 0)
        {
            BackupInfo.ShowVisualWait();

            // Контроль скопированной суммы. При отрицательном результате, ошибка.

            if (MonthBackuping(month, eias_files_lcl, simple_files_lcl, Self_Obj_Sums_in) == Self_Obj_Sums_in.All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[0]])
            {
                Backup_status_in = BackupingStatusCode.BACKUP_SUCCESS;
            }
            else
            {
                Backup_status_in = BackupingStatusCode.BACKUP_FAILURE;
                //BackupInfo.ShowCopyError();
            }
        }
        else
        {
            Backup_status_in = BackupingStatusCode.BACKUP_NOT_FOUND;
            //BackupInfo.ShowScansNotFound(month);
        }
    }
}


class YearBackupProcess : BaseBackupProcess
{
    // Список по найденным месяцам всех данных бэкапа за год.

    readonly List<(string, List<FileInfo>?, Dictionary<string, List<FileInfo>>?)> year_full_backup_in = [];

    // Эти данные нужны для логгирования всех месяцев.

    public List<(ProtocolNamesComputingPerMonth, BackupSumsPerMonth)> Full_Log_Data_in { get; } = [];

    public YearBackupProcess(DirectoryInfo source_directory, DirectoryInfo backup_directory) : base(source_directory, backup_directory)
    {
        // Продолжение процесса, если хотя бы за один месяц есть файлы.

        if (FindAllYearFiles())
        {
            // Сложение всех сумм за год, для составления отчета.

            var all_sums_lcl = ISumsTableCreator.Create(ProtocolTypesAndSums.MAIN_SUMS);
            var simple_sums_lcl = ISumsTableCreator.Create(ProtocolTypesAndSums.UNITED_SIMPLE_TYPE_SUMS);

            foreach (var month_item in Full_Log_Data_in)
            {
                foreach (var sum in month_item.Item2.All_Protocols_Sums_in)
                {
                    all_sums_lcl[sum.Key] += sum.Value;
                }

                if (month_item.Item2.Simple_Protocols_Sums_in is not null)
                {
                    foreach (var sum in month_item.Item2.Simple_Protocols_Sums_in)
                    {
                        simple_sums_lcl[sum.Key] += sum.Value;
                    }
                }
            }

            BackupInfo.ShowVisualWait();

            // Контроль сумм бэкапа. 

            if (YearBackuping() == all_sums_lcl[ProtocolTypesAndSums.MAIN_SUMS[0]])
            {
                Backup_status_in = BackupingStatusCode.BACKUP_SUCCESS;
            }
            else
            {
                Backup_status_in = BackupingStatusCode.BACKUP_FAILURE;
                //BackupInfo.ShowCopyError();
            }
        }
        else
        {
            Backup_status_in = BackupingStatusCode.BACKUP_NOT_FOUND;
            //BackupInfo.ShowScansNotFound(CurrentDate.Current_Year_Print_in);
        }
    }

    // * Поиск протоколов по всем месяцам. *

    bool FindAllYearFiles()
    {
        // Список простых протоколов ("ФФ"), предназначенный для динамического поиска неизвестных протоколов.

        List<Dictionary<string, List<FileInfo>>?> simple_files_trace_lcl = [];

        for (int month_index = 0; month_index < PeriodsNames.MONTHES.Count; month_index++)
        {
            //BackupSumsPerMonth sums_in;

            var eias_files_lcl = GetEIASFiles(CreateDatePattern(month_index));
            var simple_files_lcl = GetSimpleFiles(CreateDatePattern(month_index));

            ProtocolNamesComputingPerMonth self_obj_names_computing_lcl = new(eias_files_lcl, simple_files_lcl);

            // Добавление простых протоколов. Если NULL, то это значит что в данном месяце нет этих файлов.

            simple_files_trace_lcl.Add(simple_files_lcl);

            // За текущий месяц, кроме января, вычисляем неизвестные протоколы. 

            if (month_index != PeriodsNames.JANUARY_INDEX)
            {
                // Следовательно, если коллекция простых протоколов "simple_files_trace_lcl[month_index - 1]" равна NULL, то неизвестные не вычисляются.

                self_obj_names_computing_lcl.ComputeUnknownProtocols(simple_files_trace_lcl[month_index - 1]);
            }

            // Рассчет суммы бэкапа за текущий месяц.

            BackupSumsPerMonth self_obj_sums_lcl = new(self_obj_names_computing_lcl.Sorted_Eias_Protocol_Names_in,
                                                       self_obj_names_computing_lcl.Sorted_Simple_Protocol_Names_in,
                                                       self_obj_names_computing_lcl.Missed_Simple_Protocols_in,
                                                       self_obj_names_computing_lcl.Unknown_Simple_Protocols_in);

            // Добавление годовых данных, только если есть файлы в текущем месяце.

            if (self_obj_sums_lcl.All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[0]] != 0)
            {
                year_full_backup_in.Add((PeriodsNames.MONTHES[month_index], eias_files_lcl, simple_files_lcl));

                Full_Log_Data_in.Add((self_obj_names_computing_lcl, self_obj_sums_lcl));
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

    // * Бэкап. *

    int YearBackuping()
    {
        // Счетчик.

        int backup_count_lcl = 0;

        for (int month_index = 0; month_index < year_full_backup_in.Count; month_index++)
        {
            // Контроль сумм.
                                    
            if (MonthBackuping(year_full_backup_in[month_index].Item1, year_full_backup_in[month_index].Item2, year_full_backup_in[month_index].Item3, Full_Log_Data_in[month_index].Item2) ==
                               Full_Log_Data_in[month_index].Item2.All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[0]])
            {
                backup_count_lcl += Full_Log_Data_in[month_index].Item2.All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[0]];
            }
        }

        return backup_count_lcl;
    }
}
