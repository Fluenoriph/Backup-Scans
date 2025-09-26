// * Файл "XMLMonthLogger.cs": логгер отчета за месяц. *

using System.Xml.Linq;


class XMLMonthLogger : BaseXMLSumsData
{
    // Уровень имен протоколов.

    public XElement? Protocol_Names_Sector_in { get; }

    // Параметры: файл отчета, название месяца, объект сумм протоколов за месяц, объект имен протоколов за месяц.

    public XMLMonthLogger(XMLMonthLogFile file, string month_name, BackupSumsPerMonth backup_sums, ProtocolNamesComputingPerMonth backup_names)
    {
        var current_month_sector_lcl = file.GetMonthData(month_name);

        Sums_Sector_in = current_month_sector_lcl?.Element(XMLLogTags.SUMS);
        Protocol_Names_Sector_in = current_month_sector_lcl?.Element(XMLLogTags.PROTOCOL_NAMES);

        // Запись общих сумм.

        WriteSums(XMLLogTags.MAIN_SUMS, backup_sums.Main_Protocols_Sums_in, ProtocolTypesAndSums.MAIN_SUMS);

        // Запись имен протоколов ЕИАС, при условии, что они найдены. Т.е. их сумма не равна нулю.

        if (backup_sums.Main_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[1]] != 0)
        {
            WriteNames(XMLLogTags.MAIN_SUMS[1], backup_names.Sorted_Eias_Protocol_Names_in);
        }
        else
        {
            // Если нет файлов ЕИАС, то записываем пустую строку в этот уровень.

            WriteNames(XMLLogTags.MAIN_SUMS[1]);
        }

        // Запись простых протоколов по физ. факторам, если они найдены.

        if (backup_sums.Main_Protocols_Sums_in[ProtocolTypesAndSums.MAIN_SUMS[2]] != 0)
        {
            WriteSums(XMLLogTags.SIMPLE_PROTOCOLS_SUMS, backup_sums.Simple_Protocols_Sums_in, ProtocolTypesAndSums.UNITED_SIMPLE_TYPE_SUMS);

            // * Запись сортированных имен по возрастанию номера протокола. *
            // Проходим по всем названиям сумм, т.к. нужно записывать отсутствующие протоколы как пустую строку.

            foreach (string name in ProtocolTypesAndSums.TYPES_FULL_NAMES)
            {
                // Создание текущего тэга, по которому идет запись. 

                var target_tag_lcl = XMLLogTags.SIMPLE_PROTOCOLS_TYPES_ONLY[ProtocolTypesAndSums.TYPES_FULL_NAMES.IndexOf(name)];

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

            WriteNames(XMLLogTags.SIMPLE_PROTOCOLS_SUMS[11], backup_names.Missed_Simple_Protocols_in);
            WriteNames(XMLLogTags.SIMPLE_PROTOCOLS_SUMS[12], backup_names.Unknown_Simple_Protocols_in);
        }
        else
        {
            // Если не найдены протоколы, то записываем суммы по нулям и имена пустыми строками.

            WriteSums(XMLLogTags.SIMPLE_PROTOCOLS_SUMS);

            // Пустые сектора названий протоколов по типам.

            foreach (var tag in XMLLogTags.SIMPLE_PROTOCOLS_TYPES_ONLY)
            {
                WriteNames(tag);
            }

            // Явная запись пустых строк пропущенных и неизвестных.

            WriteNames(XMLLogTags.SIMPLE_PROTOCOLS_SUMS[11]);
            WriteNames(XMLLogTags.SIMPLE_PROTOCOLS_SUMS[12]);
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
