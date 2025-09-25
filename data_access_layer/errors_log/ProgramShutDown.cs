// * Файл "ProgramShutDown.cs": класс, логгер с "вылетом" из программы. *

class ProgramShutDown : BaseErrorReporter
{
    protected override string Factor { get; } = "| Критическая ошибка выполнения | Работа программы прекращена |";

    public ProgramShutDown(ErrorCode code, string? exception_message = null) : base(code, exception_message)
    {
        Environment.Exit(0);
    }
}
