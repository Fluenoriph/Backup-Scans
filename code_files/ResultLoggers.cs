/*
 * Файл ResultLoggers.cs: логгеры отчетов.
 * 
 * 1. "BaseXmlSumsData": базовый класс. Суммы протоколов.
 * 2. "XmlMonthLogger": логгер отчета за месяц;
 * 3. "XmlYearLogger": логгер отчета за год.
 */

using System.Globalization;
using System.Xml.Linq;


abstract class BaseXmlSumsData
{
    // Уровень сумм.

    public XElement? Sums_Sector_in { get; set; }

    // * Запись в файл, непосредственно сумм. Параметры: "tags" - текущие тэги сумм, "sums" - данные сумм, "names" - названия типов сумм. *

    protected void WriteSums(List<string> tags, Dictionary<string, int>? sums = null, List<string>? names = null)
    {
        for (int sum_index = 0; sum_index < tags.Count; sum_index++)
        {
            // Сначала получаем сектор текущей суммы.

            var current_sector_lcl = Sums_Sector_in?.Element(tags[sum_index]);

            IXMLNullError<XElement>.CheckItem(current_sector_lcl);

            // Параметры "sums" и "names" могут быть нулевыми при отсутствии каких-либо протоколов, тогда сразу записываем ноль.

            if ((sums is not null) && (names is not null))
            {
                // Сумма равна значению из словаря сумм "sums".

                current_sector_lcl!.Value = sums[names[sum_index]].ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                // Сумма равна "0".

                current_sector_lcl!.Value = Symbols.NULL;
            }
        }
    }
}


class XmlMonthLogger : BaseXmlSumsData
{
    // Уровень имен протоколов.

    public XElement? Protocol_Names_Sector_in { get; }

    // Параметры: файл отчета, название месяца, объект суммы бэкапа за месяц, протоколы ЕИАС.

    public XmlMonthLogger(MonthLogFile file, string month_name, BackupSumsPerMonth backup_sums, ProtocolNamesComputingPerMonth backup_names)
    {
        var current_month_sector_lcl = file.GetMonthData(month_name);

        Sums_Sector_in = current_month_sector_lcl?.Element(XmlTags.SUMS_TAG);
        Protocol_Names_Sector_in = current_month_sector_lcl?.Element(XmlTags.PROTOCOL_NAMES_TAG);

        // Запись общих сумм.

        WriteSums(XmlTags.MAIN_SUMS_TAGS, backup_sums.All_Protocols_Sums_in, ProtocolTypesAndSums.MAIN_SUMS);

        // Запись имен протоколов ЕИАС, при условии, что они найдены. Т.е. их сумма не равна нулю.

        if (backup_sums.All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[1]] != 0)
        {                                    
            WriteNames(XmlTags.MAIN_SUMS_TAGS[1], backup_names.Sorted_Eias_Protocol_Names_in);
        }
        else
        {
            // Если нет файлов ЕИАС, то записываем пустую строку в этот уровень.

            WriteNames(XmlTags.MAIN_SUMS_TAGS[1]);
        }

        // Запись простых протоколов по физ. факторам, если они найдены.

        if (backup_sums.All_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[2]] != 0)
        {
            WriteSums(XmlTags.SIMPLE_PROTOCOLS_SUMS_TAGS, backup_sums.Simple_Protocols_Sums_in, ProtocolTypesAndSums.UNITED_SIMPLE_TYPE_SUMS);

            // * Запись сортированных имен по возрастанию номера протокола. *
            // Проходим по всем названиям сумм, т.к. нужно записывать отсутствующие протоколы как пустую строку.

            foreach (string name in ProtocolTypesAndSums.TYPES_FULL_NAMES)
            {
                // Создание текущего тэга, по которому идет запись. 

                var target_tag_lcl = XmlTags.TYPE_TAGS[ProtocolTypesAndSums.TYPES_FULL_NAMES.IndexOf(name)];

                // Проверка, какие типы протоколов есть в словаре сортированных имен.

                if (backup_names.Sorted_Simple_Protocol_Names_in!.TryGetValue(name, out List<string>? value))
                {
                    WriteNames(target_tag_lcl, value);
                }
                else
                {
                    WriteNames(target_tag_lcl);
                }
            }

            // Запись пропущенных и неизвестных протоколов.

            WriteNames(XmlTags.SIMPLE_PROTOCOLS_SUMS_TAGS[11], backup_names.Missed_Simple_Protocols_in);
            WriteNames(XmlTags.SIMPLE_PROTOCOLS_SUMS_TAGS[12], backup_names.Unknown_Simple_Protocols_in);
        }
        else
        {
            // Если не найдены протоколы, то записываем суммы по нулям и имена пустыми строками.

            WriteSums(XmlTags.SIMPLE_PROTOCOLS_SUMS_TAGS);

            // Пустые сектора названий протоколов по типам.

            foreach (var tag in XmlTags.TYPE_TAGS)
            {
                WriteNames(tag);
            }

            // Явная запись пустых строк пропущенных и неизвестных.

            WriteNames(XmlTags.SIMPLE_PROTOCOLS_SUMS_TAGS[11]);
            WriteNames(XmlTags.SIMPLE_PROTOCOLS_SUMS_TAGS[12]);
        }

        // Сохраняем все.

        file.Document_in!.Save(file.Filename_in);
    }

    // * Запись имен протоколов. *
    // Параметры: тэг записи, список имен.

    void WriteNames(string tag, List<string>? names = null)
    {
        var current_sector_lcl = Protocol_Names_Sector_in?.Element(tag);

        IXMLNullError<XElement>.CheckItem(current_sector_lcl);
                
        // Запись имен протоколов через запятую. Если нет протоколов, то записываем пустую строку.

        if (names is not null)
        {
            current_sector_lcl!.Value = string.Join(", ", names);
        }
        else
        {
            current_sector_lcl!.Value = string.Empty;
        }
    }
}


class XmlYearLogger : BaseXmlSumsData
{
    // Параметры: "file" - файл годового отчета, "all_protocols_sums" - суммы общие, "simple_protocols_sums" - суммы протоколов по физическим факторам. 

    public XmlYearLogger(YearLogFile file, Dictionary<string, int> all_protocols_sums, Dictionary<string, int> simple_protocols_sums)
    {
        Sums_Sector_in = file.Document_in!.Element(XmlTags.SUMS_TAG);

        // Запись и сохранение.

        WriteSums(XmlTags.MAIN_SUMS_TAGS, all_protocols_sums, ProtocolTypesAndSums.MAIN_SUMS);
        WriteSums(XmlTags.SIMPLE_PROTOCOLS_SUMS_TAGS, simple_protocols_sums, ProtocolTypesAndSums.UNITED_SIMPLE_TYPE_SUMS);

        file.Document_in.Save(file.Filename_in);
    }
}
