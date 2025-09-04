// * Запись отчета о возникшей ошибке в текстовый файл. *

interface IErrorsWriter
{
    // Параметры: "factor" - общая причина ошибки, "code" - код ошибки, "exception_message" - сообщение от системы исключений.

    static void Write(string factor, ErrorCode code, string? exception_message = null)
    {
        string date_border_lcl = new(Symbols.LINE, 3);

        // Если не передаем сообщение исключения, то оцениваем ошибку по внутреннему коду.

        exception_message ??= "error_code";

        using StreamWriter errors_file = new(string.Concat(Directory.GetCurrentDirectory(), Symbols.SLASH, "errors.txt"), true);
            errors_file.WriteLine($"\n{date_border_lcl} {CurrentDate.DateAndTime} {date_border_lcl} {string.Concat(factor, $" Code: {(int)code}", $"| Report: {exception_message};")}");
    }
}
