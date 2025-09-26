// * Файл "YearBackupProcess.cs": класс для резервного копирования за год. *

class YearBackupProcess : BaseBackupProcess
{
    // Все найденные протоколы за год.

    readonly List<(List<FileInfo>?, Dictionary<string, List<FileInfo>>?)> year_full_backup_files_in = [];

    // Эти данные нужны для логгирования всех месяцев.

    public List<(string, BackupSumsPerMonth, ProtocolNamesComputingPerMonth)> Full_Log_Data_in { get; } = [];

    // Свод сумм за год.

    public Dictionary<string, int> Main_Sums_in { get; } = ISumsTableCreator.Create(ProtocolTypesAndSums.MAIN_SUMS);

    public Dictionary<string, int> Simple_Protocols_Sums_in { get; } = ISumsTableCreator.Create(ProtocolTypesAndSums.UNITED_SIMPLE_TYPE_SUMS);

    public YearBackupProcess(DirectoryInfo source_directory, DirectoryInfo backup_directory) : base(source_directory, backup_directory)
    {
        // Продолжение процесса, если хотя бы за один месяц есть файлы.

        if (FindAllYearFiles())
        {
            // Сложение всех сумм за год, для составления отчета.

            foreach (var month_item in Full_Log_Data_in)
            {
                foreach (var sum in month_item.Item2.Main_Protocols_Sums_in)
                {
                    Main_Sums_in[sum.Key] += sum.Value;
                }

                if (month_item.Item2.Simple_Protocols_Sums_in is not null)
                {
                    foreach (var sum in month_item.Item2.Simple_Protocols_Sums_in)
                    {
                        Simple_Protocols_Sums_in[sum.Key] += sum.Value;
                    }
                }
            }

            BackupInfo.ShowVisualWait();

            // Контроль сумм бэкапа. 

            if (YearBackuping() == Main_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[0]])
            {
                Backup_status_in = BackupingStatusCode.BACKUP_SUCCESS;
            }
            else
            {
                Backup_status_in = BackupingStatusCode.BACKUP_FAILURE;
            }
        }
        else
        {
            Backup_status_in = BackupingStatusCode.BACKUP_NOT_FOUND;
        }
    }

    // * Поиск протоколов по всем месяцам. *

    bool FindAllYearFiles()
    {
        // Список простых протоколов ("ФФ"), предназначенный для динамического поиска неизвестных протоколов.

        List<Dictionary<string, List<FileInfo>>?> simple_files_trace_lcl = [];

        for (int month_index = 0; month_index < Periods.MONTHES.Count; month_index++)
        {
            //BackupSumsPerMonth sums_in;

            var eias_files_lcl = GetEIASFiles(CreateDatePattern(month_index));
            var simple_files_lcl = GetSimpleFiles(CreateDatePattern(month_index));

            ProtocolNamesComputingPerMonth self_obj_names_computing_lcl = new(eias_files_lcl, simple_files_lcl);

            // Добавление простых протоколов. Если NULL, то это значит что в данном месяце нет этих файлов.

            simple_files_trace_lcl.Add(simple_files_lcl);

            // За текущий месяц, кроме января, вычисляем неизвестные протоколы. 

            if (month_index != Periods.JANUARY_INDEX)
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

            if (self_obj_sums_lcl.Main_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[0]] != 0)
            {
                year_full_backup_files_in.Add((eias_files_lcl, simple_files_lcl));

                Full_Log_Data_in.Add((Periods.MONTHES[month_index], self_obj_sums_lcl, self_obj_names_computing_lcl));
            }
        }

        // Проверка поиска.

        if (year_full_backup_files_in.Count != 0)
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

        for (int month_index = 0; month_index < year_full_backup_files_in.Count; month_index++)
        {
            // Контроль сумм.

            if (MonthBackuping(Full_Log_Data_in[month_index].Item1, year_full_backup_files_in[month_index].Item1, year_full_backup_files_in[month_index].Item2, Full_Log_Data_in[month_index].Item2) ==
                               Full_Log_Data_in[month_index].Item2.Main_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[0]])
            {
                backup_count_lcl += Full_Log_Data_in[month_index].Item2.Main_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[0]];
            }
        }

        return backup_count_lcl;
    }
}
