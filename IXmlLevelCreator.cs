using System.Xml.Linq;


interface IXmlLevelCreator
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
