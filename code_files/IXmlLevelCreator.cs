// * Файл IXmlLevelCreator.cs: интерфейс для создания уровня в XML файле. *

using System.Xml.Linq;


interface IXmlLevelCreator
{
    // Создание уровня с корневым тэгом "sector" и списком вложенных тэгов "tags".

    static XElement Create(string sector, List<string> tags)
    {
        XElement x_sector_lcl = new(sector);

        foreach (string tag in tags)
        {
            x_sector_lcl.Add(new XElement(tag));
        }

        return x_sector_lcl;
    }
}
