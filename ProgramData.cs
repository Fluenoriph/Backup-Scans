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
}


struct WorkDirectories
{
    public static Dictionary<string, string> NAMES = new()
    {
        [XmlTags.DRIVE_TAGS[0]] = "ИСХОДНАЯ",
        [XmlTags.DRIVE_TAGS[1]] = "РЕЗЕРВНАЯ",
        [XmlTags.DRIVE_TAGS[2]] = "ОТЧЕТНАЯ"
    };
}


struct LogFiles
{
    public static string DRIVES_CONFIG_FILE = string.Concat(Directory.GetCurrentDirectory(), '\\', "drives_config.xml");

    public const string MONTH_LOG_FILE = "backup_log_monthes.xml";

    public const string YEAR_LOG_FILE = "year_log.xml";
}


struct FilePatterns
{
    public const string NUMBER_GROUP = "^(?<number>";

    public static string EIAS_NUMBER_PATTERN = string.Concat(NUMBER_GROUP, "\\d{5}-\\d{2}-\\d{2})-");

    public static string SIMPLE_NUMBER_PATTERN = string.Concat(NUMBER_GROUP, "\\d{1,4})-");

    public const string PROTOCOL_SCAN_TYPE = "pdf";
}


struct PeriodsNames
{
    public const string YEAR = "Год";

    public static List<string> MONTHES = ["Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"];

    public const int JANUARY_INDEX = 0;
    public const int OCTOBER_INDEX = 9;
    public const int DECEMBER_INDEX = 11;
}


struct ProtocolTypesSums
{
    public static List<string> TYPES_FULL_NAMES = ["Физические факторы (Уссурийск)", "Физические факторы (Арсеньев)",
                                                           "Радиационный контроль (Уссурийск)", "Радиационный контроль (Арсеньев)",
                                                           "Измерения мебели (Уссурийск)", "Измерения мебели (Арсеньев)"];

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
}
