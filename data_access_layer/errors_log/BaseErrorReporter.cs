// * Файл "BaseErrorReporter.cs": базовый класс логгеров ошибок. *

abstract class BaseErrorReporter
{
    readonly string date_border_in = new(Symbols.LINE, 3);

    // Общая причина ошибки.

    protected abstract string Factor { get; }

    // Параметры: "code" - код ошибки, "exception_message" - сообщение от системы исключений.

    public BaseErrorReporter(ErrorCode code, string? exception_message)
    {
        // Если не передаем сообщение исключения, то оцениваем ошибку по внутреннему коду.

        exception_message ??= "error_code";

        using StreamWriter errors_file_lcl = new(Path.Combine(Directory.GetCurrentDirectory(), "errors.txt"), true);
        errors_file_lcl.WriteLine($"\n{date_border_in} {CurrentDate.Date_And_Time_in} {date_border_in} {string.Concat(Factor, $" Code: {(int)code}", $"| Report: {exception_message};")}");
    }
}
