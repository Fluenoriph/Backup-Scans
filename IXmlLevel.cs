using System.Xml.Linq;


interface IXmlLevel
{
    static XElement Create(string sector, List<string> tags)
    {
        XElement x_sector = new(sector);

        foreach (string tag in tags)
        {
            x_sector.Add(new XElement(tag));
        }

        return x_sector;
    }
}
