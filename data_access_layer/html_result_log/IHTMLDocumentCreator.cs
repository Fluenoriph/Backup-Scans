// * Файл "": интерфейс создания файла отчета. *

interface IHTMLDocumentCreator
{
    // * Параметры: директория рабочего пространства, все данные отчета (гипертекст), номер периода (месяц или год). *

    static void CreateLogFile(string work_space_path, string data, int period_value)
    {
        using StreamWriter writer = new(Path.Combine(work_space_path, $"{period_value}_log.html"));
        writer.WriteLine(data);
    }
}
