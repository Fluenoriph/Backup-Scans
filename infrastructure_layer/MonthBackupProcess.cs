// * "MonthBackupProcess": класс для резервного копирования за месяц;

using InfoOut;


class MonthBackupProcess : BaseBackupProcess
{
    // Интерфейс для логгирования.

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
            }
        }
        else
        {
            Backup_status_in = BackupingStatusCode.BACKUP_NOT_FOUND;
        }
    }
}
