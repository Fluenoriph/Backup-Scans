// * Файл "XMLMonthLogFile.cs": класс файла отчета за год. *

using System.Xml.Linq;


class XMLMonthLogFile(string file_path) : BaseXMLDataFile(file_path)
{
    protected override XElement Root_Sector_in { get; } = CreateMonthLevels();

    // Создание уровней по каждому месяцу.

    static XElement CreateMonthLevels()
    {
        XElement root_lcl = new(XMLLogTags.MONTH_LOG_ROOT);

        foreach (string month in Periods.MONTHES)
        {
            XElement x_month_lcl = new(XMLLogTags.MONTH);

            // Название месяца.

            XAttribute current_month_lcl = new(XMLLogTags.MONTH_NAME, month);

            // Создание секторов сумм и имен протоколов.

            x_month_lcl.Add(current_month_lcl);
            x_month_lcl.Add(IXMLLevelCreator.Create(XMLLogTags.SUMS, XMLLogTags.ALL_SUMS));
            x_month_lcl.Add(IXMLLevelCreator.Create(XMLLogTags.PROTOCOL_NAMES, [XMLLogTags.MAIN_SUMS[1], .. XMLLogTags.SIMPLE_PROTOCOLS_TYPES_ONLY, XMLLogTags.SIMPLE_PROTOCOLS_SUMS[11], XMLLogTags.SIMPLE_PROTOCOLS_SUMS[12]]));

            root_lcl.Add(x_month_lcl);
        }

        return root_lcl;
    }

    // Получение элемента (уровня) месяца по входному параметру - названию.

    public XElement? GetMonthData(string month)
    {
        return Document_in!.Element(XMLLogTags.MONTH_LOG_ROOT)?.Elements(XMLLogTags.MONTH).FirstOrDefault(x_month => x_month.Attribute(XMLLogTags.MONTH_NAME)?.Value == month);
    }
}
