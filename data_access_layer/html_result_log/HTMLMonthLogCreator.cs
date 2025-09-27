// * Файл "HTMLMonthLogCreator.cs": класс, создатель гипертекста отчета за месяц. *

class HTMLMonthLogCreator : BaseHTMLSumsLevelLog
{
    // Имена протоколов.

    readonly ProtocolNamesComputingPerMonth names_in;

    public HTMLMonthLogCreator(Dictionary<string, int> main_protocols_sums, Dictionary<string, int>? simple_protocols_sums, 
                               ProtocolNamesComputingPerMonth protocol_names, string? period = null) : base(main_protocols_sums, simple_protocols_sums, period)
    {
        names_in = protocol_names;

        // Создание заголовка секции имен протоколов.

        Log_Data_in.Add(HTMLPartialTemplates.PutProtocolNamesSectionHeader());

        // Создание секции ЕИАС протоколов, при их наличии.

        CreateSingleLevel(ProtocolTypesAndSums.MAIN_SUMS[1], protocol_names.Sorted_Eias_Protocol_Names_in);

        // Обычных протоколов.

        CreateSimpleProtocolsSection();

        // Закрытие файла.

        Log_Data_in.Add(HTMLPartialTemplates.PutEndFile());
    }

    // * Создание секций обычных протоколов, при их наличии. *

    void CreateSimpleProtocolsSection()
    {
        if (main_sums_in[ProtocolTypesAndSums.MAIN_SUMS[2]] != NULL_DIGIT)
        {
            for (int type_index = 0, location_index = 0; type_index < ProtocolTypesAndSums.SIMPLE_PROTOCOL_TYPES.Count; type_index++)
            {
                // Создаем секцию по типу, если существуют имена.

                if (simple_sums_in![ProtocolTypesAndSums.FULL_TYPE_SUMS[type_index]] != NULL_DIGIT)
                {
                    // Абзац названия типа протоколов.

                    Log_Data_in.Add(HTMLPartialTemplates.PutProtocolNamesSimpleTypeHeader(ProtocolTypesAndSums.SIMPLE_PROTOCOL_TYPES[type_index]));

                    // Имена по Уссурийску.

                    CreateLocationSection(ProtocolTypesAndSums.TYPES_FULL_NAMES[location_index], ProtocolTypesAndSums.LOCATIONS[0]);

                    // Имена по Арсеньеву.

                    CreateLocationSection(ProtocolTypesAndSums.TYPES_FULL_NAMES[location_index + 1], ProtocolTypesAndSums.LOCATIONS[1]);

                    // Закрытие секции типа.

                    Log_Data_in.Add(HTMLPartialTemplates.SIMPLE_PROTOCOL_TYPE_NAMES_SECTION_END);
                }

                location_index += 2;
            }

            // Секция пропущенных.

            CreateSingleLevel(ProtocolTypesAndSums.NOT_FOUND_SUMS[0], names_in.Missed_Simple_Protocols_in);

            // Секция неизвестных.

            CreateSingleLevel(ProtocolTypesAndSums.NOT_FOUND_SUMS[1], names_in.Unknown_Simple_Protocols_in);
        }
    }

    // * Создание одноуровневого списка протоколов. *

    void CreateSingleLevel(string protocol_types, List<string>? protocol_names)
    {
        if (protocol_names is not null)
        {
            Log_Data_in.Add(HTMLPartialTemplates.PutSingleLevelList(protocol_types, string.Join(Symbols.NAME_SEPARATOR, protocol_names)));
        }
    }

    // * Создание списка протоколов по локации. *

    void CreateLocationSection(string protocol_type, string city)
    {
        if (names_in.Sorted_Simple_Protocol_Names_in!.TryGetValue(protocol_type, out List<string>? value))
        {
            Log_Data_in.Add(HTMLPartialTemplates.PutTwoLevelList(city, string.Join(Symbols.NAME_SEPARATOR, value)));
        }
    }
}
