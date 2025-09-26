// * Файл "IRealValue.cs": интерфейс проверки значения больше нуля или не пустой строки (из текстовых данных). *

interface IRealValue
{
    static bool GetStatus(string? value)
    {
        return (value is not "") && (value is not Symbols.NULL);
    }
}
