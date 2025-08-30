interface IErrorsWriter
{
    static void Write(string message)
    {
        string date_border_lcl = new(Symbols.LINE, 3);

        using StreamWriter errors_file = new(string.Concat(Directory.GetCurrentDirectory(), Symbols.SLASH, "errors.txt"), true);
            errors_file.WriteLine($"\n{date_border_lcl} {CurrentDate.DateAndTime} {date_border_lcl} {message};");
    }
}
