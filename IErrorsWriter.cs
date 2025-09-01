interface IErrorsWriter
{
    static void Write(string factor, ErrorCodes code, string? exception_message = null)
    {
        string date_border_lcl = new(Symbols.LINE, 3);

        exception_message ??= "error_code";

        using StreamWriter errors_file = new(string.Concat(Directory.GetCurrentDirectory(), Symbols.SLASH, "errors.txt"), true);
            errors_file.WriteLine($"\n{date_border_lcl} {CurrentDate.DateAndTime} {date_border_lcl} {string.Concat(factor, $" Code: {(int)code}", $"| Report: {exception_message};")}");
    }
}
