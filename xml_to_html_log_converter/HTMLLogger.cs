using System.Xml.Linq;


// Есть другой алгоритм, добавлять в список частей лог файла, валидные данные по индексу, по порядку создания.

abstract class HTMLSumsLevelLog
{
    readonly XElement? sums_in;
    public List<string> Log_Data_in { get; } = [];

    public HTMLSumsLevelLog(XElement? sums_sector, string? period = null)
    {
        sums_in = sums_sector;
                
        // Шапка лога.

        Log_Data_in.Add(HTMLPartialTemplates.PutMainPart(period));

        // Строка "Всего". 

        CreateSumsTypeTableString(XmlTags.MAIN_SUMS_TAGS[0], ProtocolTypesAndSums.MAIN_SUMS[0]);

        // Создание строки ЕИАС суммы.

        CreateSumsTypeTableString(XmlTags.MAIN_SUMS_TAGS[1], ProtocolTypesAndSums.MAIN_SUMS[1]);

        // Сначала проверка на наличие суммы обычных протоколов, а потом создание таблицы.

        var simple_protocols_sums_lcl = GetValidSumValue(XmlTags.MAIN_SUMS_TAGS[2]);

        if (simple_protocols_sums_lcl is not null)
        {
            Log_Data_in.Add(HTMLPartialTemplates.PutString(ProtocolTypesAndSums.MAIN_SUMS[2], simple_protocols_sums_lcl));

            CreateSimpleProtocolsSumsTable();
        }
    }

    // * Получение действительной суммы протоколов. *

    string? GetValidSumValue(string sum_tag)
    {
        var sector_lcl = sums_in?.Element(sum_tag);

        // Проверка сектора xml файла.

        IXMLNullError<XElement>.CheckItem(sector_lcl);

        var sum_value = sector_lcl!.Value;

        // Проверка на значение больше 0.

        if (IRealValue.GetStatus(sum_value))
        {
            return sum_value;
        }
        else
        {
            return null;
        }
    }

    // * Создание строки таблицы по типу суммы. * 

    void CreateSumsTypeTableString(string tag, string sum_type)
    {
        var sum_lcl = GetValidSumValue(tag);

        if (sum_lcl is not null)
        {
            Log_Data_in!.Add(HTMLPartialTemplates.PutString(sum_type, sum_lcl));
        }
    }

    // * Создание секции сумм обычных протоколов (таблица). *

    void CreateSimpleProtocolsSumsTable()
    {
        // Создание заголовка.

        Log_Data_in!.Add(HTMLPartialTemplates.PutSimpleProtocolsSumsTableHeader());

        // Пропущенные.

        CreateSumsTypeTableString(XmlTags.SIMPLE_PROTOCOLS_SUMS_TAGS[11], ProtocolTypesAndSums.NOT_FOUND_SUMS[0]);

        // Неизвестные.

        CreateSumsTypeTableString(XmlTags.SIMPLE_PROTOCOLS_SUMS_TAGS[12], ProtocolTypesAndSums.NOT_FOUND_SUMS[1]);

        // Создание секций по типам протоколов.

        for (int sum_index = 0; sum_index < ProtocolTypesAndSums.SIMPLE_PROTOCOL_TYPES.Count; sum_index++)
        {
            var full_type_sum_lcl = GetValidSumValue(XmlTags.SIMPLE_PROTOCOLS_SUMS_TAGS[sum_index + 2]);

            // Создание раздела таблицы по типу протоколов.

            if (full_type_sum_lcl is not null)
            {
                Log_Data_in.Add(HTMLPartialTemplates.PutTableSectionHeader(ProtocolTypesAndSums.SIMPLE_PROTOCOL_TYPES[sum_index]));

                // Сумма всего.

                Log_Data_in.Add(HTMLPartialTemplates.PutString(ProtocolTypesAndSums.MAIN_SUMS[0], full_type_sum_lcl));

                // По г. Уссурийск

                CreateSumsTypeTableString(XmlTags.TYPE_TAGS[sum_index], ProtocolTypesAndSums.LOCATIONS[0]);

                // По г. Арсеньев.

                CreateSumsTypeTableString(XmlTags.TYPE_TAGS[sum_index + 1], ProtocolTypesAndSums.LOCATIONS[1]);
            }
        }

        // Закрытие таблицы.

        Log_Data_in.Add(HTMLPartialTemplates.PutSimpleProtocolsSumsTableEnd());
    }
}                                                                   

// * Годовой отчет. *

class HTMLYearLog : HTMLSumsLevelLog
{
    public HTMLYearLog(XElement? sums_sector, string? period = null) : base(sums_sector, period)
    {
        // Добавление закрывающих файл тэгов.

        Log_Data_in.Add(HTMLPartialTemplates.PutEndFile());
    }
}

// * Отчет за месяц. *

class HTMLMonthLog : HTMLSumsLevelLog
{
    readonly XElement? names_in;

    public HTMLMonthLog(XElement sums_sector, XElement names_sector, string? period = null) : base(sums_sector, period)
    {
        // Создание секции имен протоколов.

        names_in = names_sector;

        // Создание заголовка секции.

        Log_Data_in.Add(HTMLPartialTemplates.PutProtocolNamesSectionHeader());

        // Создание секции ЕИАС протоколов.

        CreateSingleLevel(XmlTags.MAIN_SUMS_TAGS[1], ProtocolTypesAndSums.MAIN_SUMS[1]);

        // Создание секций обычных протоколов.

        for (int type_index = 0, location_index = 0; type_index < ProtocolTypesAndSums.SIMPLE_PROTOCOL_TYPES.Count; type_index++)
        {
            var ussuriysk_names_lcl = GetRealNames(XmlTags.TYPE_TAGS[location_index]);
            var arsenyev_names_lcl = GetRealNames(XmlTags.TYPE_TAGS[location_index + 1]);

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

        CreateSingleLevel(XmlTags.SIMPLE_PROTOCOLS_SUMS_TAGS[11], ProtocolTypesAndSums.NOT_FOUND_SUMS[0]);

        // Секция неизвестных.

        CreateSingleLevel(XmlTags.SIMPLE_PROTOCOLS_SUMS_TAGS[12], ProtocolTypesAndSums.NOT_FOUND_SUMS[1]);

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
