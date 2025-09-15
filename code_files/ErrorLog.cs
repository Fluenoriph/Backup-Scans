/*
 * Файла ErrorLog.cs: отчет об ошибке.
 * 
 * 1. "BaseErrorReporter": базовый класс логгеров ошибок;
 * 2. "ErrorCode": перечисление кодов ошибок;
 * 3. "ProgramCrash": логгер ошибки;
 * 4. "ProgramShutDown": логгер с "вылетом" из программы.
 */

abstract class BaseErrorReporter
{
    readonly string date_border_in = new(Symbols.LINE, 3);

    // Общая причина ошибки.

    abstract protected string Factor { get; }

    // Параметры: "code" - код ошибки, "exception_message" - сообщение от системы исключений.

    public BaseErrorReporter(ErrorCode code, string? exception_message)
    {
        // Если не передаем сообщение исключения, то оцениваем ошибку по внутреннему коду.

        exception_message ??= "error_code";

        using StreamWriter errors_file_lcl = new(Path.Combine(Directory.GetCurrentDirectory(), "errors.txt"), true);
            errors_file_lcl.WriteLine($"\n{date_border_in} {CurrentDate.Date_And_Time_in} {date_border_in} {string.Concat(Factor, $" Code: {(int)code}", $"| Report: {exception_message};")}");
    }
}


enum ErrorCode
{
    /* 
       * 101 - Ошибка доступа к уровню XML файла;
       * 201 - Ошибка доступа к ресурсу "диска";
       * 202 - Рабочая директория не найдена;
       * 211 - Ресурс "диска" недоступен;
       * 301 - Ошибка ввода значения;
       * 501 - Ошибка копирования файла;
       * 601 - Несоответствие найденной и скопированной суммы файлов.
    */

    XML_ELEMENT_ACCESS_ERROR = 101,
    DRIVE_RESOURCE_ACCESS_ERROR = 201,
    DRIVE_DIRECTORY_NOT_FOUND_ERROR = 202,
    DRIVE_RESOURCE_UNAVAILABLE = 211,
    INPUT_VALUE_ERROR = 301,
    COPY_FILE_ERROR = 501,
    COPY_SUMS_FATAL_ERROR = 601
}


class ProgramCrash(ErrorCode code, string exception_message) : BaseErrorReporter(code, exception_message)
{
    protected override string Factor { get; } = "| Ошибка выполнения программы |";
}


class ProgramShutDown : BaseErrorReporter
{
    protected override string Factor { get; } = "| Критическая ошибка выполнения | Работа программы прекращена |";

    public ProgramShutDown(ErrorCode code, string? exception_message = null) : base(code, exception_message)
    {
        Environment.Exit(0);
    }
}
