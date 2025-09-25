// * Файл BaseXMLDataFile.cs: базовый класс, для классов файлов отчета и файла настройки директорий. *
//   Основная цель, проверка существования файла.

using System.Xml;
using System.Xml.Linq;


abstract class BaseXMLDataFile
{
    // Собственно, объект файла.

    public XDocument? Document_in { get; }

    // Имя файла в виде полного пути. Применяется извне как параметр.

    public string Filename_in { get; }

    /* "Тело" файла. Нужно для создания сего, в случае его отсутствия в заданных директориях.
       Соответственно для каждого файла создается свой корневой уровень. */

    abstract protected XElement? Root_Sector_in { get; }

    // Входной параметр: полный путь к файлу.

    public BaseXMLDataFile(string file_path)
    {
        Filename_in = file_path;
                
        // Если файл не существует, то создается заново.

        if (File.Exists(Filename_in))
        {
            try
            {
                Document_in = XDocument.Load(Filename_in);
            }
            catch (XmlException error)
            {
                _ = new ProgramShutDown(ErrorCode.XML_ELEMENT_ACCESS_ERROR, error.Message);
            }
        }
        else
        {
            Document_in = new();
            Document_in.Add(Root_Sector_in);
            Document_in.Save(Filename_in);
        }
    }
}
