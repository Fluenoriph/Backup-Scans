namespace TextData
{
    struct AppConstants
    {
        public const string drives_config_file = "C:\\Users\\Mahabhara\\source\\repos\\Fluenoriph\\Backup-Scans\\drives_config.xml"; // !! ..\xml
        public const string logs_file = "C:\\Users\\Mahabhara\\source\\repos\\Fluenoriph\\Backup-Scans\\backup_log_monthes.xml";
        
        public static List<string> drive_type = ["SOURCE", "DESTINATION"];
        
        public const string eias_number_pattern = "^(?<number>\\d{5})-(?<number>\\d{2})-(?<number>\\d{2})-";
        public const string simple_number_pattern = "^(?<number>\\d{1,4})-";
        
        public const string scan_file_type = "pdf";

        public static List<string> month_names = ["Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"];

        public static List<string> types_full_names = ["Физические факторы (Уссурийск)", "Физические факторы (Арсеньев)",
                                                           "Радиационный контроль (Уссурийск)", "Радиационный контроль (Арсеньев)",
                                                           "Измерения мебели (Уссурийск)", "Измерения мебели (Арсеньев)"];
        
        public static List<string> types_short_names = ["ф", "фа", "р", "ра", "м", "ма"];
        
        public static List<string> others_sums = ["Всего", "ЕИАС", "Простые"];
        public static List<string> others_sums_tags = ["FULL", "EIAS", "SIMPLE"];

        public static List<string> full_location_sums = ["Уссурийск всего", "Арсеньев всего"];
        
        public static List<string> full_type_sums = ["Физические факторы всего", "Радиационный контроль всего", "Измерения мебели всего"];
        
        public static List<string> not_found_sums = ["Пропущенные", "Неизвестные"];
        
        public static List<string> simple_sums_tags = ["uss", "ars", "f_all", "r_all", "m_all", "f", "fa", "r", "ra", "m", "ma", "misseds", "unknowns"];
    }
}
