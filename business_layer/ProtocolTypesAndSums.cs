// * Файл "ProtocolTypesAndSums.cs": структура для названий типов протоколов и сумм. *

struct ProtocolTypesAndSums
{
    // Тип протоколов ЕИАС.

    public const string EIAS_TYPE = "ЕИАС";

    // Типы обычных протоколов.

    public static List<string> SIMPLE_PROTOCOL_TYPES = ["Физические факторы", "Радиационный контроль", "Измерения мебели"];

    public static List<string> LOCATIONS = ["Уссурийск", "Арсеньев"];

    // Типы по локациям.

    public static List<string> TYPES_FULL_NAMES = [$"{SIMPLE_PROTOCOL_TYPES[0]} ({LOCATIONS[0]})", $"{SIMPLE_PROTOCOL_TYPES[0]} ({LOCATIONS[1]})",
                                                   $"{SIMPLE_PROTOCOL_TYPES[1]} ({LOCATIONS[0]})", $"{SIMPLE_PROTOCOL_TYPES[1]} ({LOCATIONS[1]})",
                                                   $"{SIMPLE_PROTOCOL_TYPES[2]} ({LOCATIONS[0]})", $"{SIMPLE_PROTOCOL_TYPES[2]} ({LOCATIONS[1]})"];

    // Сокращенные названия, используемые в именовании протоколов. Тип простого протокола: 1-ф, 22-р; 13-ма и т.д.

    public static List<string> TYPES_SHORT_NAMES = ["ф", "фа", "р", "ра", "м", "ма"];

    public static List<string> MAIN_SUMS = ["Всего", EIAS_TYPE, "Обычные"];

    public static List<string> FULL_LOCATION_SUMS = [$"{MAIN_SUMS[0]} {LOCATIONS[0]}", $"{MAIN_SUMS[0]} {LOCATIONS[1]}"];

    public static List<string> FULL_TYPE_SUMS = [$"{MAIN_SUMS[0]} {SIMPLE_PROTOCOL_TYPES[0]}", $"{MAIN_SUMS[0]} {SIMPLE_PROTOCOL_TYPES[1]}",
                                                 $"{MAIN_SUMS[0]} {SIMPLE_PROTOCOL_TYPES[2]}"];

    public static List<string> NOT_FOUND_SUMS = ["Пропущенные", "Неизвестные"];

    public static List<string> UNITED_SIMPLE_TYPE_SUMS = [.. FULL_LOCATION_SUMS, .. FULL_TYPE_SUMS, .. TYPES_FULL_NAMES, .. NOT_FOUND_SUMS];
}
