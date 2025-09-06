// * Файл MonthBackupSums.cs: класс для вычисления полного отчета за месяц. *

class MonthBackupSums
{
    public readonly SimpleProtocolNames? self_obj_names_in;

    // Все суммы протоколов (общие).

    public Dictionary<string, int> All_Protocols_Sums_in { get; } = ISumsTableCreator.Create(ProtocolTypesAndSums.OTHERS_SUMS);

    // Суммы простых протоколов по физ. факторам.

    public Dictionary<string, int>? Simple_Protocols_Sums_in { get; }

    // Параметры: ЕИАС сканы, ФФ сканы, ФФ сканы за предыдущий месяц (для вычисления неизвестных).

    public MonthBackupSums(List<FileInfo>? eias_files, Dictionary<string, List<FileInfo>>? simple_files, Dictionary<string, List<FileInfo>>? previous_period_simple_files = null)
    {
        // Если есть ЕИАС, то считаем.

        if (eias_files is not null)
        {
            // Добавление ко всей сумме протоколов.

            All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[0]] += eias_files.Count;

            // Сумма ЕИАС.

            All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[1]] = eias_files.Count;
        }

        // Если есть ФФ, то считаем.

        if (simple_files is not null)
        {
            // Создаем словарь сумм.

            Simple_Protocols_Sums_in = ISumsTableCreator.Create(ProtocolTypesAndSums.UNITED_SIMPLE_TYPE_SUMS);

            // Рассчет сумм внутренних типов протоколов и суммы всех ФФ.

            foreach (var item in simple_files)
            {
                Simple_Protocols_Sums_in![item.Key] = item.Value.Count;
                All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[2]] += item.Value.Count;
            }

            // Добавление ко всей сумме.

            All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[0]] += All_Protocols_Sums_in[ProtocolTypesAndSums.OTHERS_SUMS[2]];
                   
            // Остальные вычисления.

            CalcProtocolTypeFullSum();            
            CalcProtocolLocationSums();

            // Вычисления имен протоколов.

            self_obj_names_in = new(simple_files);
                        
            if (previous_period_simple_files is not null)
            {
                self_obj_names_in.ComputeUnknownProtocols(previous_period_simple_files);
            }

            CalcNotFoundProtocolsSums();
        }
    }

    // * Рассчет общих сумм типов. *

    void CalcProtocolTypeFullSum()
    {
        for (int type_index = 0, calc_index = 0; type_index < ProtocolTypesAndSums.FULL_TYPE_SUMS.Count; type_index++)
        {
            Simple_Protocols_Sums_in![ProtocolTypesAndSums.FULL_TYPE_SUMS[type_index]] = Simple_Protocols_Sums_in[ProtocolTypesAndSums.TYPES_FULL_NAMES[calc_index]] + Simple_Protocols_Sums_in[ProtocolTypesAndSums.TYPES_FULL_NAMES[calc_index + 1]];
            calc_index += 2;
        }
    }

    // * Рассчет сумм по локации. *

    void CalcProtocolLocationSums()
    {
        for (int city_index = 0, calc_index = 0; city_index < ProtocolTypesAndSums.FULL_LOCATION_SUMS.Count; city_index++)
        {
            Simple_Protocols_Sums_in![ProtocolTypesAndSums.FULL_LOCATION_SUMS[city_index]] = Simple_Protocols_Sums_in[ProtocolTypesAndSums.TYPES_FULL_NAMES[calc_index]] + Simple_Protocols_Sums_in[ProtocolTypesAndSums.TYPES_FULL_NAMES[calc_index + 2]] + Simple_Protocols_Sums_in[ProtocolTypesAndSums.TYPES_FULL_NAMES[calc_index + 4]];
            calc_index += 1;
        }
    }

    // * Рассчет сумм пропущенных и неизвестных протоколов, если они есть. *

    void CalcNotFoundProtocolsSums()
    {
        if (self_obj_names_in!.Missed_Protocols_in is not null)
        {
            Simple_Protocols_Sums_in![ProtocolTypesAndSums.NOT_FOUND_SUMS[0]] = self_obj_names_in.Missed_Protocols_in.Count;
        }

        if (self_obj_names_in.Unknown_Protocols_in is not null)
        {
            Simple_Protocols_Sums_in![ProtocolTypesAndSums.NOT_FOUND_SUMS[1]] = self_obj_names_in.Unknown_Protocols_in.Count;
        }
    }
}
