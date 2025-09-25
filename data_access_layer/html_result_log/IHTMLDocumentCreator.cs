interface IHTMLDocumentCreator
{
    static void CreateLogFile(string path, string data, int period_value)
    {
        using StreamWriter writer = new(Path.Combine(path, $"{period_value}_log.html"));
        writer.WriteLine(data);
    }
}
