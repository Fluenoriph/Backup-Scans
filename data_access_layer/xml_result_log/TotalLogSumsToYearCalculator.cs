// * Файл TotalLogSumsToYearCalculator.cs: класс для вычисления годового отчета из файла месячных логов. *
//   Он используется, когда делается бэкап за декабрь, для подведения итогов. Гипотетически, предыдущие месяца уже рассчитаны.

using System.Globalization;
using System.Xml.Linq;


sealed class TotalLogSumsToYearCalculator
{
    // Счетчик отдельной суммы.

    int sum_count_in;

    readonly XMLMonthLogFile self_obj_month_log_file_in;
    readonly XMLYearLogFile self_obj_year_log_file_in;

    // Список рассчитанных сумм за год, в порядке, определенном списком тэгов.

    public List<int> calculated_sums_in = [];

    // Параметры: файл месячного лога, файл годового лога.

    public TotalLogSumsToYearCalculator(XMLMonthLogFile month_log_file, XMLYearLogFile year_log_file)
    {
        self_obj_month_log_file_in = month_log_file;
        self_obj_year_log_file_in = year_log_file;

        // Рассчет по тэгу (одному типу суммы).

        foreach (string sum_tag in XMLLogTags.ALL_SUMS)
        {
            sum_count_in = 0;

            // Вычисляем результат за все месяцы.

            foreach (string month_name in Periods.MONTHES)
            {
                AddSum(month_name, sum_tag);
            }

            WriteYearSum(sum_tag);

            // Добавление рассчитанной суммы в список.

            calculated_sums_in.Add(sum_count_in);
        }

        year_log_file.Document_in!.Save(year_log_file.Filename_in);
    }

    // Суммирование. Параметры: название месяца, название тэга.
   
    void AddSum(string month_name, string sum_tag)
    {
        // Получение значения суммы.

        var sum_value_lcl = self_obj_month_log_file_in.GetMonthData(month_name)?.Element(XMLLogTags.SUMS)?.Element(sum_tag)?.Value;

        IXMLNullError<string>.CheckItem(sum_value_lcl);

        // Если значение не "0" и не пустая строка, то суммируем.
               
        if (IRealValue.GetStatus(sum_value_lcl))
        {
            sum_count_in += Convert.ToInt32(sum_value_lcl!, CultureInfo.CurrentCulture);
        }
    }

    // Запись в файл годового отчета. Параметры: тэг суммы.
    
    void WriteYearSum(string sum_tag)
    {
        // Получение и изменение (запись) значения суммы.

        var sum_value_lcl = self_obj_year_log_file_in.Document_in!.Element(XMLLogTags.SUMS)?.Element(sum_tag);

        IXMLNullError<XElement>.CheckItem(sum_value_lcl);

        sum_value_lcl!.Value = sum_count_in.ToString(CultureInfo.CurrentCulture);
    } 
}
