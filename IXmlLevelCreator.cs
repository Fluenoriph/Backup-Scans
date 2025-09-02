using System.Xml.Linq;


// * Интерфейс применяется для создания сектора в XML файле. *

interface IXmlLevelCreator
{
    // Создание уровня с корневым тэгом "sector" и списком вложенных тэгов "tags".

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
