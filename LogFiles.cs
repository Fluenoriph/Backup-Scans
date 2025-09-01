using System.Xml.Linq;


class YearLogFile(string file_path) : XmlDataFile(file_path)
{
    private protected override XElement Root_Sector_in { get; } = IXmlLevelCreator.Create(XmlTags.SUMS_TAG, XmlTags.UNITED_SUMS_TAGS);
}


class MonthLogFile(string file_path) : XmlDataFile(file_path)
{
    private protected override XElement Root_Sector_in { get; } = CreateMonthLevels();

    private static XElement CreateMonthLevels()
    {
        XElement root = new(XmlTags.MONTH_LOG_ROOT_TAG);

        foreach (string month in PeriodsNames.MONTHES)
        {
            XElement x_month = new(XmlTags.MONTH_TAG);
            XAttribute current_month = new(XmlTags.MONTH_NAME_TAG, month);

            x_month.Add(current_month);
            x_month.Add(IXmlLevelCreator.Create(XmlTags.SUMS_TAG, XmlTags.UNITED_SUMS_TAGS));
            x_month.Add(IXmlLevelCreator.Create(XmlTags.PROTOCOL_NAMES_TAG, [XmlTags.OTHERS_SUMS_TAGS[1], .. XmlTags.TYPE_TAGS, XmlTags.SIMPLE_SUMS_TAGS[11], XmlTags.SIMPLE_SUMS_TAGS[12]]));

            root.Add(x_month);
        }

        return root;
    }

    public XElement? GetMonthData(string month)
    {
        return Document_in!.Element(XmlTags.MONTH_LOG_ROOT_TAG)?.Elements(XmlTags.MONTH_TAG).FirstOrDefault(x_month => x_month.Attribute(XmlTags.MONTH_NAME_TAG)?.Value == month);
    }
}
