// * Файл "BaseHTMLSumsLevelLog.cs": базовый класс для классов, конструкторов отчета. *

using System.Globalization;


abstract class BaseHTMLSumsLevelLog
{
    protected readonly int NULL_DIGIT = Convert.ToInt32(Symbols.NULL, CultureInfo.CurrentCulture);
    protected readonly Dictionary<string, int> main_sums_in;
    protected readonly Dictionary<string, int>? simple_sums_in;

    public List<string> Log_Data_in { get; } = [];

    public BaseHTMLSumsLevelLog(Dictionary<string, int> main_protocols_sums, Dictionary<string, int>? simple_protocols_sums, string? period = null)
    {
        main_sums_in = main_protocols_sums;
        simple_sums_in = simple_protocols_sums;

        // Шапка лога.

        Log_Data_in.Add(HTMLPartialTemplates.PutMainPart(period));

        // Строка "Всего". 

        CreateSumsTypeTableString(main_sums_in, ProtocolTypesAndSums.MAIN_SUMS[0]);

        // Создание строки ЕИАС суммы.

        CreateSumsTypeTableString(main_sums_in, ProtocolTypesAndSums.MAIN_SUMS[1]);

        // Сначала проверка на наличие суммы обычных протоколов, а потом создание таблицы.

        if (main_sums_in[ProtocolTypesAndSums.MAIN_SUMS[2]] != NULL_DIGIT)
        {
            Log_Data_in.Add(HTMLPartialTemplates.PutTableString(ProtocolTypesAndSums.MAIN_SUMS[2],
                            main_sums_in[ProtocolTypesAndSums.MAIN_SUMS[2]].ToString(CultureInfo.CurrentCulture)));

            Log_Data_in.Add(HTMLPartialTemplates.PutMainSumsTableEnd());

            CreateSimpleProtocolsSumsTable();
        }
        else
        {
            Log_Data_in.Add(HTMLPartialTemplates.PutSumsTableSectionEnd());
        }
    }

    // Создание строки таблицы по типу суммы, если она больше нуля. 

    void CreateSumsTypeTableString(Dictionary<string, int> current_sums, string sum_type)
    {
        if (current_sums[sum_type] != NULL_DIGIT)
        {
            Log_Data_in.Add(HTMLPartialTemplates.PutTableString(sum_type, current_sums[sum_type].ToString(CultureInfo.CurrentCulture)));
        }
    }

    // Создание таблицы сумм обычных протоколов.

    void CreateSimpleProtocolsSumsTable()
    {
        // Создание заголовка.

        Log_Data_in.Add(HTMLPartialTemplates.PutSimpleProtocolsSumsTableHeader());

        // Пропущенные.

        CreateSumsTypeTableString(simple_sums_in!, ProtocolTypesAndSums.NOT_FOUND_SUMS[0]);

        // Неизвестные.

        CreateSumsTypeTableString(simple_sums_in!, ProtocolTypesAndSums.NOT_FOUND_SUMS[1]);

        // Создание секций по типам протоколов.

        for (int type_index = 0, location_index = 0; type_index < ProtocolTypesAndSums.SIMPLE_PROTOCOL_TYPES.Count; type_index++)
        {
            // Создание раздела таблицы по типу протоколов, если они присутствуют.

            if (simple_sums_in![ProtocolTypesAndSums.FULL_TYPE_SUMS[type_index]] != NULL_DIGIT)
            {
                Log_Data_in.Add(HTMLPartialTemplates.PutTableSectionHeader(ProtocolTypesAndSums.SIMPLE_PROTOCOL_TYPES[type_index]));

                // Сумма всего.

                Log_Data_in.Add(HTMLPartialTemplates.PutTableString(ProtocolTypesAndSums.MAIN_SUMS[0], 
                    simple_sums_in[ProtocolTypesAndSums.FULL_TYPE_SUMS[type_index]].ToString(CultureInfo.CurrentCulture)));

                // По г. Уссурийск

                CreateLocationSumTableString(ProtocolTypesAndSums.TYPES_FULL_NAMES[location_index], ProtocolTypesAndSums.LOCATIONS[0]);

                // По г. Арсеньев.

                CreateLocationSumTableString(ProtocolTypesAndSums.TYPES_FULL_NAMES[location_index + 1], ProtocolTypesAndSums.LOCATIONS[1]);
            }

            location_index += 2;
        }

        // Закрытие таблицы.

        Log_Data_in.Add(HTMLPartialTemplates.PutSumsTableSectionEnd());


        // Создание строки таблицы по локации, при наличии протоколов.

        void CreateLocationSumTableString(string sum_type, string city)
        {
            if (simple_sums_in[sum_type] != NULL_DIGIT)
            {
                Log_Data_in.Add(HTMLPartialTemplates.PutTableString(city, simple_sums_in[sum_type].ToString(CultureInfo.CurrentCulture)));
            }
        }
    }
}