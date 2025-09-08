/* 
 * Файл "ProgramData.cs": ресурсы, используемые в программе.
 * 
 * 1. "DrivesConfigFileLocation": структура для создания пути файла настройки рабочих директорий;
 * 2. "FilePatterns": структура паттернов, для создания регулярных выражений;
 * 3. "LogFilesNames": структура файлов отчетов;
 * 4. "PeriodsNames": структура временных периодов;
 * 5. "ProtocolTypesAndSums": структура типов протоколов и сумм;
 * 6. "Symbols": структура символов, используемых в программе;
 * 7. "XmlTags": структура тэгов XML файлов.
 */

struct DrivesConfigFileLocation
{
    // По умолчанию, создается в расположении исполняемого файла программы.

    // Пренебрегаем обработкой исключения "UnauthorizedAccessException". Оно возникает при попытке создать этот файл, в заблокированном расположении.

    public static string full_program_path = string.Concat(Directory.GetCurrentDirectory(), Symbols.SLASH, "drives_config.xml");
}


struct FilePatterns
{
    public const string NUMBER_GROUP = "^(?<number>";

    // Номер ЕИАС протокола. Пример: 12345-01-25. 

    public static string EIAS_NUMBER_PATTERN = string.Concat(NUMBER_GROUP, "\\d{5}-\\d{2}-\\d{2})-");

    // Номер простого протокола. Пример: 1; 10; 100; 1000.

    public static string SIMPLE_NUMBER_PATTERN = string.Concat(NUMBER_GROUP, "\\d{1,4})-");

    public const string PROTOCOL_SCAN_FILE_TYPE = "pdf";
}


struct LogFilesNames
{
    public const string MONTH_LOG_FILE = "backup_log_monthes.xml";

    public const string YEAR_LOG_FILE = "year_log.xml";
}


struct PeriodsNames
{
    public const string YEAR = "Год";

    public static List<string> MONTHES = ["Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"];

    public const int JANUARY_INDEX = 0;
    public const int OCTOBER_INDEX = 9;
    public const int DECEMBER_INDEX = 11;
}


struct ProtocolTypesAndSums
{
    // Ниже, сокращенные названия, используемые в именовании протоколов.

    public static List<string> TYPES_FULL_NAMES = ["Физические факторы (Уссурийск)", "Физические факторы (Арсеньев)",
                                                           "Радиационный контроль (Уссурийск)", "Радиационный контроль (Арсеньев)",
                                                           "Измерения мебели (Уссурийск)", "Измерения мебели (Арсеньев)"];

    // Тип простого протокола: 1-ф, 22-р; 13-ма и т.д.

    public static List<string> TYPES_SHORT_NAMES = ["ф", "фа", "р", "ра", "м", "ма"];

    public static List<string> OTHERS_SUMS = ["Всего", "ЕИАС", "Простые"];

    public static List<string> FULL_LOCATION_SUMS = ["Уссурийск всего", "Арсеньев всего"];

    public static List<string> FULL_TYPE_SUMS = ["Физические факторы всего", "Радиационный контроль всего", "Измерения мебели всего"];

    public static List<string> NOT_FOUND_SUMS = ["Пропущенные", "Неизвестные"];

    public static List<string> UNITED_SIMPLE_TYPE_SUMS = [.. FULL_LOCATION_SUMS, .. FULL_TYPE_SUMS, .. TYPES_FULL_NAMES, .. NOT_FOUND_SUMS];
}


struct Symbols
{
    public const char LINE = '-';

    public const char SLASH = '\\';

    public const char STAR = '*';

    public const char GRILLE = '#';

    public const string NULL = "0";

    public const string CHANGE_DIRECTORY_FUNCTION = "d";

    public const string FLOW_RIGHT = ">>>";

    public const int NOT_DRIVE_INDEX = -1;

    public const int EIAS_NUMBER_COUNT = 9;
}


struct XmlTags
{
    public static List<string> DRIVE_TAGS = ["SOURCE", "DESTINATION", "XML_LOG"];

    public const string DRIVES_CONFIG_TAG = "configuration";

    public const string SUMS_TAG = "sums";

    public const string MONTH_LOG_ROOT_TAG = "logs_data";

    public const string MONTH_TAG = "month";

    public const string PROTOCOL_NAMES_TAG = "protocol_names";

    public const string MONTH_NAME_TAG = "name";

    public static List<string> OTHERS_SUMS_TAGS = ["full", "eias", "simple"];

    public static List<string> SIMPLE_SUMS_TAGS = ["uss", "ars", "f_all", "r_all", "m_all", "f", "fa", "r", "ra", "m", "ma", "misseds", "unknowns"];

    public static List<string> TYPE_TAGS = SIMPLE_SUMS_TAGS.GetRange(5, 6);

    public static List<string> UNITED_SUMS_TAGS = [.. OTHERS_SUMS_TAGS, .. SIMPLE_SUMS_TAGS];
}
