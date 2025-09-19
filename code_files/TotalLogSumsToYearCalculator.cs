// * Файл TotalLogSumsToYearCalculator.cs: класс для вычисления годового отчета из файла месячного лога. *
//   Он используется, когда делается бэкап за декабрь. Гипотетически, предыдущие месяца уже рассчитаны.

using System.Globalization;
using System.Xml.Linq;


class TotalLogSumsToYearCalculator
{
    // Счетчик суммы

    int sum_count_in;

    readonly MonthLogFile self_obj_month_log_file_in;
    readonly YearLogFile self_obj_year_log_file_in;

    // Список рассчитанных сумм за год, в порядке, определенном тэгами.

    readonly List<int> calculated_sums_in = [];

    // Параметры: файл месячного лога, файл годового лога.

    public TotalLogSumsToYearCalculator(MonthLogFile month_log_file, YearLogFile year_log_file)
    {
        self_obj_month_log_file_in = month_log_file;
        self_obj_year_log_file_in = year_log_file;

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

    void AddSum(string month_name, string sum_tag)
    {
        // Получение значения суммы.

        var sum_value_lcl = self_obj_month_log_file_in.GetMonthData(month_name)?.Element(XmlTags.SUMS_TAG)?.Element(sum_tag)?.Value;

        IXMLNullError<string>.CheckItem(sum_value_lcl);

        // Если значение не "0" и не пустая строка, то суммируем.
               
        if (IRealValue.GetStatus(sum_value_lcl))
        {
            sum_count_in += Convert.ToInt32(sum_value_lcl!, CultureInfo.CurrentCulture);
        }
    }

    // * Запись в файл годового отчета. *
    // Параметры: тэг вида суммы.

    void WriteYearSum(string sum_tag)
    {
        // Получение и изменение (запись) значения суммы.

        var sum_value_lcl = self_obj_year_log_file_in.Document_in!.Element(XmlTags.SUMS_TAG)?.Element(sum_tag);

        IXMLNullError<XElement>.CheckItem(sum_value_lcl);

        sum_value_lcl!.Value = sum_count_in.ToString(CultureInfo.CurrentCulture);
    }

    // * Получение годовых сумм в формате, необходимом для вывода в консоль. *

    public (Dictionary<string, int>, Dictionary<string, int>?) GetYearSums()
    {
        // Создание словаря общих сумм. 

        var all_protocol_sums_lcl = ISumsTableCreator.Create(ProtocolTypesAndSums.MAIN_SUMS);

        for (int sum_index = 0; sum_index < all_protocol_sums_lcl.Count; sum_index++)
        {
            all_protocol_sums_lcl[ProtocolTypesAndSums.MAIN_SUMS[sum_index]] = calculated_sums_in.GetRange(0, 3)[sum_index];
        }

        // Простых протоколов может и не быть. Если они есть, то создаем словарь.

        Dictionary<string, int>? simple_protocol_sums_lcl = null;

        if (calculated_sums_in[2] != 0)
        {
            simple_protocol_sums_lcl = ISumsTableCreator.Create(ProtocolTypesAndSums.UNITED_SIMPLE_TYPE_SUMS);

            for (int sum_index = 0; sum_index < simple_protocol_sums_lcl.Count; sum_index++)
            {
                simple_protocol_sums_lcl[ProtocolTypesAndSums.UNITED_SIMPLE_TYPE_SUMS[sum_index]] = calculated_sums_in.GetRange(3, 13)[sum_index];
            }
        }

        return (all_protocol_sums_lcl, simple_protocol_sums_lcl);
    }
}
