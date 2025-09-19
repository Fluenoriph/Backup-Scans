using System.Xml.Linq;


// Пока главная линия логики.

abstract class HTMLSumsLevelLog
{
    readonly XElement? sums_in;
    public List<string>? Log_Data_in { get; }

    public HTMLSumsLevelLog(XElement? sums, string? period = null)
    {
        sums_in = sums;

        var full_protocol_sums_lcl = GetValidSumValue(XmlTags.MAIN_SUMS_TAGS[0]);

        if (full_protocol_sums_lcl is not null)
        {
            Log_Data_in = [];

            Log_Data_in.Add(HTMLPartialTemplates.PutMainPart(period));

            // Создание строки ЕИАС суммы.

            CreateSumsTypeTableString(XmlTags.MAIN_SUMS_TAGS[1], ProtocolTypesAndSums.MAIN_SUMS[1]);

            var simple_protocols_sums_lcl = GetValidSumValue(XmlTags.MAIN_SUMS_TAGS[2]);

            if (simple_protocols_sums_lcl is not null)
            {
                Log_Data_in.Add(HTMLPartialTemplates.PutString(ProtocolTypesAndSums.MAIN_SUMS[2], simple_protocols_sums_lcl));

                CreateSimpleProtocolsSumsTable();
            }
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
                Log_Data_in.Add(HTMLPartialTemplates.PutTableSectionHeader(ProtocolTypesAndSums.SIMPLE_PROTOCOL_TYPES[0]));

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


class HTMLLogger
{
    
    readonly XElement? names_in;

    

    public HTMLLogger(XElement? sums, XElement names, DirectoryInfo log_directory)
    {
        
        names_in = names;


        






        
        

       
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

    



    // * Создание секции имен протоколов. *

    void CreateProtocolNamesSection()
    {
        // Создание одноуровневого списка протоколов.

        void CreateSingleLevel(string protocol_names_tag, string protocol_type)
        {
            var names_lcl = GetRealNames(protocol_names_tag);

            if (names_lcl is not null)
            {
                log_data.Add(HTMLPartialTemplates.PutSingleLevelList(protocol_type, names_lcl));
            }
        }

        // Создание заголовка секции.

        log_data.Add(HTMLPartialTemplates.PutProtocolNamesSectionHeader());

        // Создание секции ЕИАС протоколов.

        CreateSingleLevel(XmlTags.MAIN_SUMS_TAGS[1], ProtocolTypesAndSums.MAIN_SUMS[0]);






    }
}
