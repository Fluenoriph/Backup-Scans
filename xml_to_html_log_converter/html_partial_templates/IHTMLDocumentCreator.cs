using Aspose.Html;


interface IHTMLDocumentCreator
{
    static void CreateLogFile(string path, string data, string period_value)
    {
        using HTMLDocument doc = new(data, ".");
            doc.Save(Path.Combine(path, $"{period_value}_log.html"));
    }
}
