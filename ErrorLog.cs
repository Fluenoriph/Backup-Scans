enum ErrorCodes
{
    XML_ELEMENT_ACCESS_ERROR = 101,
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


class ProgramStop
{
    public ProgramStop(ErrorCodes code, string exception_message)
    {
        IErrorsWriter.Write($"| Program_Stop | Error_Code: {(int)code} | Report: {exception_message}");
    }
}
