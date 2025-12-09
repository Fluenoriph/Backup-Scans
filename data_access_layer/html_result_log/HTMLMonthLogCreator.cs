// * Файл "HTMLMonthLogCreator.cs": класс, создатель гипертекста отчета за месяц. *

sealed class HTMLMonthLogCreator : BaseHTMLSumsLevelLog
{
    // Имена протоколов.

    readonly ProtocolNamesComputingPerMonth names_in;

    public HTMLMonthLogCreator(Dictionary<string, int> main_protocols_sums, Dictionary<string, int>? simple_protocols_sums, 
                               ProtocolNamesComputingPerMonth protocol_names, string? period = null) : base(main_protocols_sums, simple_protocols_sums, period)
    {
        names_in = protocol_names;

        // Создание заголовка секции имен протоколов.

        Log_Data_in.Add(HTMLPartialTemplates.PutProtocolNamesSectionHeader());

        // Создание раздела имен ЕИАС протоколов, при их наличии.

        CreateSingleProtocolsContainer(ProtocolTypesAndSums.MAIN_SUMS[1], protocol_names.Sorted_Eias_Protocol_Names_in);

        // >> Обычных протоколов.

        CreateSimpleProtocolsSection();

        // Закрытие файла.

        Log_Data_in.Add(HTMLPartialTemplates.PutEndFile());
    }

    // Создание раздела обычных протоколов, при их наличии.

    void CreateSimpleProtocolsSection()
    {
        if (main_sums_in[ProtocolTypesAndSums.MAIN_SUMS[2]] != NULL_DIGIT)
        {
            Log_Data_in.Add(HTMLPartialTemplates.PutStartDivision());

            for (int type_index = 0, location_index = 0; type_index < ProtocolTypesAndSums.SIMPLE_PROTOCOL_TYPES.Count; type_index++)
            {
                // Создаем сектор типа, если существуют имена.

                if (simple_sums_in![ProtocolTypesAndSums.FULL_TYPE_SUMS[type_index]] != NULL_DIGIT)
                {
                    // Заголовок названия типа протоколов.

                    Log_Data_in.Add(HTMLPartialTemplates.PutHeaderThirdLevel(ProtocolTypesAndSums.SIMPLE_PROTOCOL_TYPES[type_index]));

                    // Имена по Уссурийску.

                    CreateLocationParagraph(ProtocolTypesAndSums.TYPES_FULL_NAMES[location_index], ProtocolTypesAndSums.LOCATIONS[0]);

                    // Имена по Арсеньеву.

                    CreateLocationParagraph(ProtocolTypesAndSums.TYPES_FULL_NAMES[location_index + 1], ProtocolTypesAndSums.LOCATIONS[1]);
                }

                location_index += 2;
            }

            // Секция пропущенных.

            CreateSingleProtocolsContainer(ProtocolTypesAndSums.NOT_FOUND_SUMS[0], names_in.Missed_Simple_Protocols_in);

            // Секция неизвестных.

            CreateSingleProtocolsContainer(ProtocolTypesAndSums.NOT_FOUND_SUMS[1], names_in.Unknown_Simple_Protocols_in);
        }
    }

    // Создание контейнера имен протоколов одного отдельного типа.

    void CreateSingleProtocolsContainer(string protocol_types, List<string>? protocol_names)
    {
        if (protocol_names is not null)
        {
            Log_Data_in.Add(HTMLPartialTemplates.PutStartDivision());
            Log_Data_in.Add(HTMLPartialTemplates.PutHeaderThirdLevel(protocol_types));
            Log_Data_in.Add(HTMLPartialTemplates.PutSingleLevelList(string.Join(Symbols.NAME_SEPARATOR, protocol_names)));
            Log_Data_in.Add(HTMLPartialTemplates.PutEndDivision());
        }
    }

    // Создание списка протоколов по локации.

    void CreateLocationParagraph(string protocol_type, string city)
    {
        if (names_in.Sorted_Simple_Protocol_Names_in!.TryGetValue(protocol_type, out List<string>? value))
        {
            Log_Data_in.Add(HTMLPartialTemplates.PutParagraph($"{city}:"));
            Log_Data_in.Add(HTMLPartialTemplates.PutSingleLevelList(string.Join(Symbols.NAME_SEPARATOR, value)));
        }
    }
}
