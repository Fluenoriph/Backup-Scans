// * Файл "ProgramCrash.cs": класс, логгер ошибки. *

sealed class ProgramCrash(ErrorCode code, string exception_message) : BaseErrorReporter(code, exception_message)
{
    protected override string Factor { get; } = "| Ошибка выполнения программы |";
}
