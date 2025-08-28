using System.Xml.Linq;


struct SumsSector
{
    public static List<string> tags = [.. XmlTags.OTHERS_SUMS_TAGS, .. XmlTags.SIMPLE_SUMS_TAGS];
    public static XElement sums = IXmlLevel.Create(XmlTags.SUMS_TAG, tags);
}
