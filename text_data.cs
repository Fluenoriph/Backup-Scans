namespace TextData
{
    struct AppConstants
    {
        public const string drives_config_file = "C:\\Users\\Mahabhara\\source\\repos\\Fluenoriph\\Backup-Scans\\drives_config.xml"; // !! ..\xml
        
        public const string month_logs_file = "C:\\Users\\Mahabhara\\source\\repos\\Fluenoriph\\Backup-Scans\\backup_log_monthes.xml";
        public const string year_log_file = "C:\\Users\\Mahabhara\\source\\repos\\Fluenoriph\\Backup-Scans\\year_log.xml";

        public static List<string> drive_type = ["SOURCE", "DESTINATION"];
        
        public const string eias_number_pattern = "^(?<number>\\d{5}-\\d{2}-\\d{2})-";
        public const string simple_number_pattern = "^(?<number>\\d{1,4})-";
        
        public const string scan_file_type = "pdf";

        public const string year = "год";
        public static List<string> month_names = ["Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"];

        public static List<string> types_full_names = ["Физические факторы (Уссурийск)", "Физические факторы (Арсеньев)",
                                                           "Радиационный контроль (Уссурийск)", "Радиационный контроль (Арсеньев)",
                                                           "Измерения мебели (Уссурийск)", "Измерения мебели (Арсеньев)"];
        
        public static List<string> types_short_names = ["ф", "фа", "р", "ра", "м", "ма"];
        
        public static List<string> others_sums = ["Всего", "ЕИАС", "Простые"];
        public static List<string> others_sums_tags = ["full", "eias", "simple"];

        public static List<string> full_location_sums = ["Уссурийск всего", "Арсеньев всего"];
        
        public static List<string> full_type_sums = ["Физические факторы всего", "Радиационный контроль всего", "Измерения мебели всего"];
        
        public static List<string> not_found_sums = ["Пропущенные", "Неизвестные"];
        
        public static List<string> simple_sums_tags = ["uss", "ars", "f_all", "r_all", "m_all", "f", "fa", "r", "ra", "m", "ma", "misseds", "unknowns"];

        public const char line = '-';

        public static string text_line = new('<', 15);
    }


    class ConsoleLogOut(Dictionary<string, int> all_sums, Dictionary<string, int>? simple_sums)
    {
        private readonly Action<string> AllSumsLineLogOut = (sum_name) => Console.WriteLine($"> {sum_name}: {all_sums[sum_name]}");
        private readonly Action<string> SimpleSumsLineLogOut = (sum_name) => Console.WriteLine($"> {sum_name}: {simple_sums![sum_name]}");

        public void ShowLog()
        {
            static void SeparateString()
            {
                Console.WriteLine(new string('=', 45));
            }

            SeparateString();
            AllSumsLineLogOut(AppConstants.others_sums[0]);
            AllSumsLineLogOut(AppConstants.others_sums[1]);
            SeparateString();
            AllSumsLineLogOut(AppConstants.others_sums[2]);

            if (simple_sums is not null)
            {
                AppConstants.full_location_sums.ForEach(name => SimpleSumsLineLogOut(name));
                SeparateString();
                SimpleSumsLineLogOut(AppConstants.full_type_sums[0]);
                SimpleSumsLineLogOut(AppConstants.types_full_names[0]);
                SimpleSumsLineLogOut(AppConstants.types_full_names[1]);
                SeparateString();
                SimpleSumsLineLogOut(AppConstants.full_type_sums[1]);
                SimpleSumsLineLogOut(AppConstants.types_full_names[2]);
                SimpleSumsLineLogOut(AppConstants.types_full_names[3]);
                SeparateString();
                SimpleSumsLineLogOut(AppConstants.full_type_sums[2]);
                SimpleSumsLineLogOut(AppConstants.types_full_names[4]);
                SimpleSumsLineLogOut(AppConstants.types_full_names[5]);
                SeparateString();
                AppConstants.not_found_sums.ForEach(name => SimpleSumsLineLogOut(name));
            }            
            SeparateString();
        }
    }




}
