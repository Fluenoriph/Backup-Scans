// * Файл "": интерфейс создания файла отчета. *

interface IHTMLDocumentCreator
{
    // * Параметры: директория рабочего пространства, все данные отчета (гипертекст), номер периода (месяц или год). *

    static void CreateLogFile(string work_space_path, string data, string? period_value = null)
    {
        string period;

        if (period_value is not null)
        {
            period = period_value;
        }
        else
        {
            period = "year";
        }

        using StreamWriter writer = new(Path.Combine(work_space_path, string.Concat(period, "_log.html")));
        writer.WriteLine(data);
    }
}
