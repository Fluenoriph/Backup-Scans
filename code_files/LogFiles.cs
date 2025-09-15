/*
 * Файл LogFiles.cs: файлы отчетов.
 * 
 * 1. "MonthLogFile": класс файла отчета за год;
 * 2. "YearLogFile": класс файла отчета за год.
 */

using System.Xml.Linq;


class MonthLogFile(string file_path) : BaseXmlDataFile(file_path)
{
    protected override XElement Root_Sector_in { get; } = CreateMonthLevels();

    // Создание уровней по каждому месяцу.

    static XElement CreateMonthLevels()
    {
        XElement root_lcl = new(XmlTags.MONTH_LOG_ROOT_TAG);

        foreach (string month in PeriodsNames.MONTHES)
        {
            XElement x_month_lcl = new(XmlTags.MONTH_TAG);

            // Название месяца.

            XAttribute current_month_lcl = new(XmlTags.MONTH_NAME_TAG, month);

            // Создание секторов сумм и имен протоколов.

            x_month_lcl.Add(current_month_lcl);
            x_month_lcl.Add(IXmlLevelCreator.Create(XmlTags.SUMS_TAG, XmlTags.UNITED_SUMS_TAGS));
            x_month_lcl.Add(IXmlLevelCreator.Create(XmlTags.PROTOCOL_NAMES_TAG, [XmlTags.MAIN_SUMS_TAGS[1], .. XmlTags.TYPE_TAGS, XmlTags.SIMPLE_PROTOCOLS_SUMS_TAGS[11], XmlTags.SIMPLE_PROTOCOLS_SUMS_TAGS[12]]));

            root_lcl.Add(x_month_lcl);
        }

        return root_lcl;
    }

    // Получение элемента (уровня) месяца по входному параметру - названию.

    public XElement? GetMonthData(string month)
    {
        return Document_in!.Element(XmlTags.MONTH_LOG_ROOT_TAG)?.Elements(XmlTags.MONTH_TAG).FirstOrDefault(x_month => x_month.Attribute(XmlTags.MONTH_NAME_TAG)?.Value == month);
    }
}


class YearLogFile(string file_path) : BaseXmlDataFile(file_path)
{
    // Корневой уровень, предназначенный для создания файла, если он не существует, по указанным путям.

    protected override XElement Root_Sector_in { get; } = IXmlLevelCreator.Create(XmlTags.SUMS_TAG, XmlTags.UNITED_SUMS_TAGS);
}
