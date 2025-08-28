using Aspose.Words;
using System.Runtime.InteropServices.Marshalling;

enum ErrorsCodes
{
    XML_ELEMENT_ACCESS_ERROR = 101,
    INPUT_VALUE_ERROR = 301,
    COPY_FILES_ERROR = 501
}


class ErrorLog
{
    private static readonly string date_border_in = new(Symbols.LINE, 3);
    private static ErrorsCodes code_in;

    public ErrorLog(ErrorsCodes code)   
    {
        code_in = code;

        using StreamWriter errors_file = new(string.Concat(Directory.GetCurrentDirectory(), '\\', "errors.txt"), true);    
            errors_file.WriteLine($"{date_border_in} {CurrentDate.DateAndTime} {date_border_in} {LogErrorType()}");

        Environment.Exit(0);
    }

    private protected virtual string LogErrorType()
    {
        return $"| Error Type: {code_in};";
    }
}
