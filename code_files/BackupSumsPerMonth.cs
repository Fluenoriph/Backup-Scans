// * Файл BackupSumsPerMonth.cs: класс для вычисления всех сумм отчета за месяц. *

class BackupSumsPerMonth
{
    // Все суммы протоколов (общие).

    public Dictionary<string, int> All_Protocols_Sums_in { get; } = ISumsTableCreator.Create(ProtocolTypesAndSums.MAIN_SUMS);

    // Суммы простых протоколов по физ. факторам.

    public Dictionary<string, int>? Simple_Protocols_Sums_in { get; }

    // Параметры: ЕИАС сортированные сканы, ФФ сортированные сканы, пропущенные ФФ, неизвестные ФФ.

    public BackupSumsPerMonth(List<string>? sorted_eias_protocols, Dictionary<string, List<string>>? sorted_simple_protocols, List<string>? missed_simples, List<string>? unknown_simples)
    {
        // Если есть ЕИАС, то считаем.

        if (sorted_eias_protocols is not null)
        {
            // Добавление ко всей сумме протоколов.

            All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[0]] += sorted_eias_protocols.Count;

            // Сумма ЕИАС.

            All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[1]] = sorted_eias_protocols.Count;
        }

        // Если есть ФФ, то считаем.

        if (sorted_simple_protocols is not null)
        {
            // Создаем словарь сумм.

            Simple_Protocols_Sums_in = ISumsTableCreator.Create(ProtocolTypesAndSums.UNITED_SIMPLE_TYPE_SUMS);

            // Рассчет сумм внутренних типов протоколов и суммы всех ФФ.

            foreach (var item in sorted_simple_protocols)
            {
                Simple_Protocols_Sums_in![item.Key] = item.Value.Count;
                All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[2]] += item.Value.Count;
            }

            // Добавление ко всей сумме.

            All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[0]] += All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[2]];
                   
            // Вычисления сумм типов обычных протоколов.

            CalcProtocolTypeFullSum();            
            CalcProtocolLocationSums();

            // Рассчет сумм пропущенных и неизвестных, если они есть (обычные).           

            if (missed_simples is not null)
            {
                Simple_Protocols_Sums_in![ProtocolTypesAndSums.NOT_FOUND_SUMS[0]] = missed_simples.Count;
            }

            if (unknown_simples is not null)
            {
                Simple_Protocols_Sums_in![ProtocolTypesAndSums.NOT_FOUND_SUMS[1]] = unknown_simples.Count;
            }
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
}
