using System.Xml.Linq;


abstract class LogFile
{
    public XDocument Document_in { get; }
    public string Filename_in { get; }

    abstract private protected XElement? Root_Sector_in { get; }

    public LogFile(string file_path)
    {
        Filename_in = file_path;

        FileInfo file_lcl = new(file_path);

        if (file_lcl.Exists)
        {
            Document_in = XDocument.Load(file_lcl.FullName);
        }
        else
        {
            Document_in = new();
            Document_in.Add(Root_Sector_in);
            Document_in.Save(file_lcl.FullName);
        }
    }
}


class DrivesConfigurationFile(string file_path) : LogFile(file_path) 
{
    private protected override XElement Root_Sector_in { get; } = IXmlLevel.Create(XmlTags.DRIVES_CONFIG_TAG, XmlTags.DRIVE_TAGS);
}


class YearLogFile(string file_path) : LogFile(file_path)
{
    private protected override XElement Root_Sector_in { get; } = SumsSector.sums;
}


class MonthLogFile(string file_path) : LogFile(file_path)
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
            x_month.Add(SumsSector.sums);
            x_month.Add(IXmlLevel.Create(XmlTags.PROTOCOL_NAMES_TAG, [XmlTags.OTHERS_SUMS_TAGS[1], .. XmlTags.TYPE_TAGS, XmlTags.SIMPLE_SUMS_TAGS[11], XmlTags.SIMPLE_SUMS_TAGS[12]]));

            root.Add(x_month);
        }

        return root;
    }

    public XElement? GetMonthData(string month)
    {
        return Document_in.Root?.Elements(XmlTags.MONTH_TAG).FirstOrDefault(x_month => x_month.Attribute(XmlTags.MONTH_NAME_TAG)?.Value == month);
    }
}
