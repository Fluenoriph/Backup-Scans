// * Файл "BaseXMLSumsData.cs": базовый класс для логгеров отчета. Уровень сумм протоколов. *

using System.Globalization;
using System.Xml.Linq;


abstract class BaseXMLSumsData
{
    // Уровень сумм.

    public XElement? Sums_Sector_in { get; set; }

    // Запись в файл, непосредственно сумм. Параметры: "tags" - текущие тэги сумм, "sums" - данные сумм, "names" - названия типов сумм.

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
