using System.Xml.Linq;


                                                           









// * Годовой отчет. *

class HTMLYearLog : BaseHTMLSumsLevelLog
{
    public HTMLYearLog(XElement? sums_sector, string? period = null) : base(sums_sector, period)
    {
        // Добавление закрывающих файл тэгов.

        Log_Data_in.Add(HTMLPartialTemplates.PutEndFile());
    }
}

// * Отчет за месяц. *

class HTMLMonthLog : BaseHTMLSumsLevelLog
{
    readonly XElement? names_in;

    public HTMLMonthLog(XElement sums_sector, XElement names_sector, string? period = null) : base(sums_sector, period)
    {
        // Создание секции имен протоколов.

        names_in = names_sector;

        // Создание заголовка секции.

        Log_Data_in.Add(HTMLPartialTemplates.PutProtocolNamesSectionHeader());

        // Создание секции ЕИАС протоколов.

        CreateSingleLevel(XMLLogTags.MAIN_SUMS[1], ProtocolTypesAndSums.MAIN_SUMS[1]);

        // Создание секций обычных протоколов.

        for (int type_index = 0, location_index = 0; type_index < ProtocolTypesAndSums.SIMPLE_PROTOCOL_TYPES.Count; type_index++)
        {
            var ussuriysk_names_lcl = GetRealNames(XMLLogTags.SIMPLE_PROTOCOLS_TYPES_ONLY[location_index]);
            var arsenyev_names_lcl = GetRealNames(XMLLogTags.SIMPLE_PROTOCOLS_TYPES_ONLY[location_index + 1]);

            bool type_names_exist = !((ussuriysk_names_lcl is null) && (arsenyev_names_lcl is null));

            // Создаем секцию по типу, если существуют имена.

            if (type_names_exist)
            {
                // Абзац названия типа протоколов.

                Log_Data_in.Add(HTMLPartialTemplates.PutProtocolNamesSimpleTypeHeader(ProtocolTypesAndSums.SIMPLE_PROTOCOL_TYPES[type_index]));

                // Имена по Уссурийску.

                CreateLocationSection(ussuriysk_names_lcl, ProtocolTypesAndSums.LOCATIONS[0]);

                // Имена по Арсеньеву.

                CreateLocationSection(arsenyev_names_lcl, ProtocolTypesAndSums.LOCATIONS[1]);

                // Закрытие секции типа.

                Log_Data_in.Add(HTMLPartialTemplates.SIMPLE_PROTOCOL_TYPE_NAMES_SECTION_END);
            }

            location_index += 2;
        }

        // Секция пропущенных.

        CreateSingleLevel(XMLLogTags.SIMPLE_PROTOCOLS_SUMS[11], ProtocolTypesAndSums.NOT_FOUND_SUMS[0]);

        // Секция неизвестных.

        CreateSingleLevel(XMLLogTags.SIMPLE_PROTOCOLS_SUMS[12], ProtocolTypesAndSums.NOT_FOUND_SUMS[1]);

        // Закрытие файла.

        Log_Data_in.Add(HTMLPartialTemplates.PutEndFile());
    }

    // * Получение имен протоколов, если они есть. *

    string? GetRealNames(string names_tag)
    {
        var sector_lcl = names_in?.Element(names_tag);

        IXMLNullError<XElement>.CheckItem(sector_lcl);

        var names_list_lcl = sector_lcl!.Value;

        if (names_list_lcl is not "")
        {
            return names_list_lcl;
        }
        else
        {
            return null;
        }
    }

    // * Создание одноуровневого списка протоколов. *

    void CreateSingleLevel(string protocol_names_tag, string protocol_type)
    {
        var names_lcl = GetRealNames(protocol_names_tag);

        if (names_lcl is not null)
        {
            Log_Data_in.Add(HTMLPartialTemplates.PutSingleLevelList(protocol_type, names_lcl));
        }
    }

    // * Создание списка протоколов по локации. *

    void CreateLocationSection(string? protocol_names, string city)
    {
        if (protocol_names is not null)
        {
            Log_Data_in.Add(HTMLPartialTemplates.PutTwoLevelList(city, protocol_names));
        }
    }
}
