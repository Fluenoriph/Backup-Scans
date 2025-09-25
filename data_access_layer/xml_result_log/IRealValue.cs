// * Файл "IRealValue.cs": интерфейс проверки значения больше нуля (из текстовых данных). *

interface IRealValue
{
    static bool GetStatus(string? value)
    {
        return (value is not Symbols.NULL) && (value is not "");
    }
}
