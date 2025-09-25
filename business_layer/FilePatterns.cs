// * Файл "FilePatterns.cs": структура для паттернов регулярных выражений. *

struct FilePatterns
{
    public const string NUMBER_GROUP = "^(?<number>";

    // Номер ЕИАС протокола. Пример: 12345-01-25. 

    public static string EIAS_NUMBER_PATTERN = string.Concat(NUMBER_GROUP, "\\d{5}-\\d{2}-\\d{2})-");

    // Номер простого протокола. Пример: 1; 10; 100; 1000.

    public static string SIMPLE_NUMBER_PATTERN = string.Concat(NUMBER_GROUP, "\\d{1,4})-");

    public const string PROTOCOL_SCAN_FILE_TYPE = "pdf";
}
