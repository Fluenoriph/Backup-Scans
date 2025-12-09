// * Файл "XMLYearLogFile.cs": класс файла отчета за год. *

using System.Xml.Linq;


sealed class XMLYearLogFile(string file_path) : BaseXMLDataFile(file_path)
{
    // Корневой уровень, предназначенный для создания файла, если он не существует, по указанным путям.

    protected override XElement Root_Sector_in { get; } = IXMLLevelCreator.Create(XMLLogTags.SUMS, XMLLogTags.ALL_SUMS);
}
