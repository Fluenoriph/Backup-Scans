// * Файл "BaseBackupProcess.cs": базовый класс, для классов месячного и годового процесса копирования. *

using System.Globalization;
using System.Text.RegularExpressions;


// Параметры: исходная директория, резервная директория.

abstract class BaseBackupProcess(DirectoryInfo source_directory, DirectoryInfo destination_directory)
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

        if (month_index < Periods.OCTOBER_INDEX)
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
                backup_files[file_index].CopyTo(Path.Combine(destination_directory.CreateSubdirectory(month_and_type_subdir).FullName, backup_files[file_index].Name), true);

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

        if (sums.Main_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[1]] != 0)
        {
            // Контроль сумм, найденных и скопированных.

            if (CopyBackupFiles(eias_files!, Path.Join(current_month, ProtocolTypesAndSums.MAIN_SUMS[1])) == sums.Main_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[1]])
            {
                backup_count_lcl += sums.Main_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[1]];
            }
        }

        // Копирование, при условии, что есть "ФФ" сканы в этом месяце.

        if (sums.Main_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[2]] != 0)
        {
            // Контроль сумм.

            if (CopySimpleBlock(simple_files!, current_month) == sums.Main_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[2]])
            {
                backup_count_lcl += sums.Main_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[2]];
            }
        }

        return backup_count_lcl;
    }
}
