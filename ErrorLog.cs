enum ErrorCodes
{
    XML_ELEMENT_ACCESS_ERROR = 101,
    DRIVE_RESOURCE_ACCESS_ERROR = 201,
    INPUT_VALUE_ERROR = 301,
    COPY_FILES_ERROR = 501,
    COPY_SUMS_FATAL_ERROR = 601
}


class ProgramShutDown
{
    public ProgramShutDown(ErrorCodes code)
    {
        IErrorsWriter.Write($"| Critical_Error | Program_Shut_Down | Code: {(int)code}");

        Environment.Exit(0);
    }
}


class ProgramCrash
{
    public ProgramCrash(ErrorCodes code, string exception_message)
    {
        IErrorsWriter.Write($"| Runtime_Error | Error_Code: {(int)code} | Report: {exception_message}");
    }
}
