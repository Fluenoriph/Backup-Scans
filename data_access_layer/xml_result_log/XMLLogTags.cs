// * Файл "XMLLogTags.cs": используемые тэги. *

struct XMLLogTags
{
    public const string SUMS = "sums";

    public const string MONTH_LOG_ROOT = "logs_data";

    public const string MONTH = "month";

    public const string PROTOCOL_NAMES = "protocol_names";

    public const string MONTH_NAME = "name";

    public static List<string> MAIN_SUMS = ["full", "eias", "simple"];

    public static List<string> SIMPLE_PROTOCOLS_SUMS = ["uss", "ars", "f_all", "r_all", "m_all", "f", "fa", "r", "ra", "m", "ma", "misseds", "unknowns"];

    // Для специфических задач.

    public static List<string> SIMPLE_PROTOCOLS_TYPES_ONLY = SIMPLE_PROTOCOLS_SUMS.GetRange(5, 6);

    public static List<string> ALL_SUMS = [.. MAIN_SUMS, .. SIMPLE_PROTOCOLS_SUMS];
}
