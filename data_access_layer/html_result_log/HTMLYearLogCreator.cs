// * Файл "": класс, создатель гипертекста отчета за год. *

class HTMLYearLogCreator : BaseHTMLSumsLevelLog
{
    public HTMLYearLogCreator(Dictionary<string, int> main_protocols_sums, Dictionary<string, int>? simple_protocols_sums, string? period = null) 
        : base(main_protocols_sums, simple_protocols_sums, period)
    {
        // Добавление закрывающих файл тэгов.

        Log_Data_in.Add(HTMLPartialTemplates.PutEndFile());
    }
}
