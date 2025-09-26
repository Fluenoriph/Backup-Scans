// * Файл "WorkSpacesConfigFile.cs": класс файла настройки рабочих директорий;

using System.Xml.Linq;


class WorkSpacesConfigFile(string file_path) : BaseXMLDataFile(file_path)
{
    protected override XElement Root_Sector_in { get; } = IXMLLevelCreator.Create(XMLWorkSpacesTags.ROOT, XMLWorkSpacesTags.SPACE_TYPE);
}
