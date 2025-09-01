enum ErrorCodes
{
    XML_ELEMENT_ACCESS_ERROR = 101,
    DRIVE_RESOURCE_ACCESS_ERROR = 201,
    DRIVE_DIRECTORY_NOT_FOUND_ERROR = 202,
    DRIVE_RESOURCE_UNAVAILABLE = 211, 
    INPUT_VALUE_ERROR = 301,
    COPY_FILES_ERROR = 501,
    COPY_SUMS_FATAL_ERROR = 601
}


abstract class ErrorReporter
{
    abstract private protected string Factor { get; }

    public ErrorReporter(ErrorCodes code, string? exception_message)
    {
        IErrorsWriter.Write(Factor, code, exception_message);
    }
}


class ProgramShutDown : ErrorReporter
{
    private protected override string Factor { get; } = "| Critical_Error | Program_Shut_Down |";

    public ProgramShutDown(ErrorCodes code, string? exception_message = null) : base(code, exception_message)
    {
        Environment.Exit(0);
    }
}


class ProgramCrash(ErrorCodes code, string exception_message) : ErrorReporter(code, exception_message)
{
    private protected override string Factor { get; } = "| Runtime_Error |";
}
