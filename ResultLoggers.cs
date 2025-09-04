using System.Globalization;
using System.Xml.Linq;


// * Базовый класс, для логгеров отчетов за месяц и год. Суммы протоколов. *

abstract class SumsData
{
    // Уровень сумм.

    private protected XElement? Sums_Sector_in { get; set; }

    // * Запись в файл, непосредственно сумм. Параметры: "tags" - текущие тэги сумм, "sums" - данные сумм, "names" - названия типов сумм. *

    private protected void WriteSums(List<string> tags, Dictionary<string, int>? sums = null, List<string>? names = null)
    {
        for (int sum_index = 0; sum_index < tags.Count; sum_index++)
        {
            // Сначала получаем сектор текущей суммы.

            var current_sector_lcl = Sums_Sector_in?.Element(tags[sum_index]);

            if (current_sector_lcl is null)
            {
                _ = new ProgramShutDown(ErrorCode.XML_ELEMENT_ACCESS_ERROR);
            }

            // Параметры "sums" и "names" могут быть нулевыми, т.к. при отсутствии каких-либо протоколов, в файл сразу записываем ноль.

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


// * Логгер отчета за год. *

class YearLogger : SumsData
{
    // Параметры: "file" - файл годового отчета, "all_protocols_sums" - суммы общие, "simple_protocols_sums" - суммы протоколов по физическим факторам. 

    public YearLogger(YearLogFile file, Dictionary<string, int> all_protocols_sums, Dictionary<string, int> simple_protocols_sums)
    {
        Sums_Sector_in = file.Document_in!.Element(XmlTags.SUMS_TAG);  

        // Запись и сохранение.

        WriteSums(XmlTags.OTHERS_SUMS_TAGS, all_protocols_sums, ProtocolTypesSums.OTHERS_SUMS);
        WriteSums(XmlTags.SIMPLE_SUMS_TAGS, simple_protocols_sums, ProtocolTypesSums.UNITED_SIMPLE_TYPE_SUMS);

        file.Document_in.Save(file.Filename_in);
    }
}


// * Логгер отчета за месяц. *

class MonthLogger : SumsData
{
    // Уровень имен протоколов.

    private XElement? Protocol_Names_Sector_in { get; set; }

    // Параметры: файл отчета, название месяца, объект суммы бэкапа за месяц, протоколы ЕИАС.

    public MonthLogger(MonthLogFile file, string month_name, MonthBackupSums backup_sums, List<FileInfo>? eias_files)
    {
        var current_month_sector_lcl = file.GetMonthData(month_name);

        Sums_Sector_in = current_month_sector_lcl?.Element(XmlTags.SUMS_TAG);
        Protocol_Names_Sector_in = current_month_sector_lcl?.Element(XmlTags.PROTOCOL_NAMES_TAG);

        // Запись общих сумм.

        WriteSums(XmlTags.OTHERS_SUMS_TAGS, backup_sums.All_Protocols_Sums_in, ProtocolTypesSums.OTHERS_SUMS);

        // Запись имен протоколов ЕИАС, при условии, что они найдены. Т.е. их сумма не равна нулю.

        if (backup_sums.All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[1]] != 0)
        {
            // Сортировка по возрастанию номера протокола и запись имен.

            EIASConvert number_convert_lcl = new();
            EIASSort name_sort_lcl = new();

            WriteNames(XmlTags.OTHERS_SUMS_TAGS[1], name_sort_lcl.Sorting(number_convert_lcl.ConvertToNumbers(eias_files!), eias_files!));
        }
        else
        {
            // Если нет файлов ЕИАС, то записываем пустую строку в этот уровень.

            WriteNames(XmlTags.OTHERS_SUMS_TAGS[1]);
        }

        // Запись простых протоколов по физ. факторам, если они найдены.

        if (backup_sums.All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[2]] != 0)
        {
            WriteSums(XmlTags.SIMPLE_SUMS_TAGS, backup_sums.Simple_Protocols_Sums_in, ProtocolTypesSums.UNITED_SIMPLE_TYPE_SUMS);

            // * Запись сортированных имен по возрастанию номера протокола. *
            // Проходим по всем названиям сумм, т.к. нужно записывать отсутствующие протоколы как пустую строку.

            foreach (string name in ProtocolTypesSums.TYPES_FULL_NAMES)
            {
                // Создание текущего тэга, по которому идет запись. 

                var target_tag_lcl = XmlTags.TYPE_TAGS[ProtocolTypesSums.TYPES_FULL_NAMES.IndexOf(name)];

                // Проверка, какие типы протоколов есть в словаре сортированных имен.

                if (backup_sums.names_in!.Sorted_Names_in.TryGetValue(name, out List<string>? value))
                {
                    WriteNames(target_tag_lcl, value);
                }
                else
                {
                    WriteNames(target_tag_lcl);
                }
            }

            // Запись пропущенных и неизвестных протоколов.

            WriteNames(XmlTags.SIMPLE_SUMS_TAGS[11], backup_sums.names_in!.Missed_Protocols_in);
            WriteNames(XmlTags.SIMPLE_SUMS_TAGS[12], backup_sums.names_in!.Unknown_Protocols_in);
        }
        else
        {
            // Если не найдены протоколы, то записываем суммы по нулям и имена пустыми строками.

            WriteSums(XmlTags.SIMPLE_SUMS_TAGS);

            // Пустые сектора названий протоколов по типам.

            foreach (var tag in XmlTags.TYPE_TAGS)
            {
                WriteNames(tag);
            }

            // Пустые пропущенные и неизвестные.

            WriteNames(XmlTags.SIMPLE_SUMS_TAGS[11]);
            WriteNames(XmlTags.SIMPLE_SUMS_TAGS[12]);
        }

        // Сохраняем все.

        file.Document_in!.Save(file.Filename_in);
    }

    // * Запись имен протоколов. *
    // Параметры: тэг записи, список имен.

    private void WriteNames(string tag, List<string>? names = null)
    {
        var current_sector_lcl = Protocol_Names_Sector_in?.Element(tag);

        if (current_sector_lcl is null)
        {
            _ = new ProgramShutDown(ErrorCode.XML_ELEMENT_ACCESS_ERROR);
        }
        
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


// * Класс, предназначенный для вычисления годового отчета из файла месячного лога. *
// Он используется, когда делается бэкап за декабрь. Гипотетически, предыдущие месяца уже рассчитаны.

class TotalLogSumsToYearCalculator
{
    // Счетчик суммы

    private int sum_count_in;

    private readonly MonthLogFile month_log_file_in;
    private readonly YearLogFile year_log_file_in;

    // Список рассчитанных сумм за год, в порядке, определенном тэгами.

    private readonly List<int> calculated_sums_in = [];

    // Параметры: файл месячного лога, файл годового лога.

    public TotalLogSumsToYearCalculator(MonthLogFile month_log_file, YearLogFile year_log_file)
    {
        month_log_file_in = month_log_file;
        year_log_file_in = year_log_file;

        // Рассчет по тэгу (одному типу суммы).

        foreach (string sum_tag in XmlTags.UNITED_SUMS_TAGS)
        {
            sum_count_in = 0;
                       
            // Вычисляем результат за все месяцы.

            foreach (string month_name in PeriodsNames.MONTHES)
            {
                AddSum(month_name, sum_tag);
            }

            WriteYearSum(sum_tag);
                        
            // Добавление рассчитанной суммы в список.

            calculated_sums_in.Add(sum_count_in);
        }

        year_log_file.Document_in!.Save(year_log_file.Filename_in);
    }

    // * Суммирование. *
    // Параметры: название месяца, название тэга.

    private void AddSum(string month_name, string sum_tag)
    {
        // Получение значения суммы.

        var sum_value_lcl = month_log_file_in.GetMonthData(month_name)?.Element(XmlTags.SUMS_TAG)?.Element(sum_tag)?.Value;

        if (sum_value_lcl is null)
        {
            _ = new ProgramShutDown(ErrorCode.XML_ELEMENT_ACCESS_ERROR);
        }

        // Если значение не "0" и не пустая строка, то суммируем.

        bool real_value_status = (sum_value_lcl is not Symbols.NULL) && (sum_value_lcl is not "");   

        if (real_value_status)
        {
            sum_count_in += Convert.ToInt32(sum_value_lcl!, CultureInfo.CurrentCulture);
        }
    }

    // * Запись в файл годового отчета. *
    // Параметры: тэг вида суммы.

    private void WriteYearSum(string sum_tag)
    {
        // Получение и изменение (запись) значения суммы.

        var sum_value_lcl = year_log_file_in.Document_in!.Element(XmlTags.SUMS_TAG)?.Element(sum_tag);

        if (sum_value_lcl is not null)
        {
            sum_value_lcl.Value = sum_count_in.ToString(CultureInfo.CurrentCulture);
        }
        else
        {
            _ = new ProgramShutDown(ErrorCode.XML_ELEMENT_ACCESS_ERROR);
        }
    }

    // * Получение годовых сумм в формате, необходимом для вывода в консоль. *

    public (Dictionary<string, int>, Dictionary<string, int>?) GetYearSums()
    {
        // Создание словаря общих сумм. 

        var all_protocol_sums_lcl = ISumsTableCreator.Create(ProtocolTypesSums.OTHERS_SUMS);

        for (int sum_index = 0; sum_index < all_protocol_sums_lcl.Count; sum_index++)
        {
            all_protocol_sums_lcl[ProtocolTypesSums.OTHERS_SUMS[sum_index]] = calculated_sums_in.GetRange(0, 3)[sum_index];
        }

        // Простых протоколов может и не быть. Если они есть, то создаем словарь.

        Dictionary<string, int>? simple_protocol_sums_lcl = null;

        if (calculated_sums_in[2] != 0)
        {
            simple_protocol_sums_lcl = ISumsTableCreator.Create(ProtocolTypesSums.UNITED_SIMPLE_TYPE_SUMS);

            for (int sum_index = 0; sum_index < simple_protocol_sums_lcl.Count; sum_index++)
            {
                simple_protocol_sums_lcl[ProtocolTypesSums.UNITED_SIMPLE_TYPE_SUMS[sum_index]] = calculated_sums_in.GetRange(3, 13)[sum_index];
            }
        }

        return (all_protocol_sums_lcl, simple_protocol_sums_lcl);
    }
}
