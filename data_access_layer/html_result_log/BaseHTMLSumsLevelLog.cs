// * Файл "BaseHTMLSumsLevelLog.cs": базовый класс для классов, конструкторов отчета. *

using System.Linq;
using System.Xml.Linq;


abstract class BaseHTMLSumsLevelLog
{
    const int NULL_DIGIT = Convert.ToInt32(Symbols.NULL);
    Dictionary<string, int> united_sums;

    public List<string> Log_Data_in { get; } = [];

    public BaseHTMLSumsLevelLog(BackupSumsPerMonth backup_sums, string? period = null)
    {
        united_sums = backup_sums.Main_Protocols_Sums_in.Concat(backup_sums.Simple_Protocols_Sums_in);

        // Шапка лога.

        Log_Data_in.Add(HTMLPartialTemplates.PutMainPart(period));

        // Строка "Всего". 

        CreateSumsTypeTableString(ProtocolTypesAndSums.MAIN_SUMS[0]);

        // Создание строки ЕИАС суммы.

        CreateSumsTypeTableString(ProtocolTypesAndSums.MAIN_SUMS[1]);

        // Сначала проверка на наличие суммы обычных протоколов, а потом создание таблицы.

        int simple_protocols_count_lcl = united_sums[ProtocolTypesAndSums.MAIN_SUMS[2]];

        if (simple_protocols_count_lcl != NULL_DIGIT)
        {
            Log_Data_in.Add(HTMLPartialTemplates.PutString(ProtocolTypesAndSums.MAIN_SUMS[2], simple_protocols_count_lcl.ToString()));

            CreateSimpleProtocolsSumsTable();
        }
    }

    // * Создание строки таблицы по типу суммы, если она больше нуля. * 

    void CreateSumsTypeTableString(string sum_type)
    {
        int sum_value = united_sums[sum_type];

        if (sum_value != NULL_DIGIT)
        {
            Log_Data_in!.Add(HTMLPartialTemplates.PutString(sum_type, sum_type.ToString());
        }
    }

    // * Создание секции сумм обычных протоколов (таблица). *

    void CreateSimpleProtocolsSumsTable()
    {
        // Создание заголовка.

        Log_Data_in!.Add(HTMLPartialTemplates.PutSimpleProtocolsSumsTableHeader());

        // Пропущенные.

        CreateSumsTypeTableString(ProtocolTypesAndSums.NOT_FOUND_SUMS[0]);

        // Неизвестные.

        CreateSumsTypeTableString(ProtocolTypesAndSums.NOT_FOUND_SUMS[1]);

        // Создание секций по типам протоколов.

        for (int sum_index = 0; sum_index < ProtocolTypesAndSums.SIMPLE_PROTOCOL_TYPES.Count; sum_index++)
        {
            var full_type_sum_lcl = GetValidSumValue(XMLLogTags.SIMPLE_PROTOCOLS_SUMS[sum_index + 2]);

            // Создание раздела таблицы по типу протоколов.

            if (full_type_sum_lcl is not null)
            {
                Log_Data_in.Add(HTMLPartialTemplates.PutTableSectionHeader(ProtocolTypesAndSums.SIMPLE_PROTOCOL_TYPES[sum_index]));

                // Сумма всего.

                Log_Data_in.Add(HTMLPartialTemplates.PutString(ProtocolTypesAndSums.MAIN_SUMS[0], full_type_sum_lcl));

                // По г. Уссурийск

                CreateSumsTypeTableString(XMLLogTags.SIMPLE_PROTOCOLS_TYPES_ONLY[sum_index], ProtocolTypesAndSums.LOCATIONS[0]);

                // По г. Арсеньев.

                CreateSumsTypeTableString(XMLLogTags.SIMPLE_PROTOCOLS_TYPES_ONLY[sum_index + 1], ProtocolTypesAndSums.LOCATIONS[1]);
            }
        }

        // Закрытие таблицы.

        Log_Data_in.Add(HTMLPartialTemplates.PutSimpleProtocolsSumsTableEnd());
    }
}