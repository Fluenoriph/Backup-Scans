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

    public static List<string> UNITED_SIMPLE_TYPE_NAMES = [.. FULL_LOCATION_SUMS, .. FULL_TYPE_SUMS, .. TYPES_FULL_NAMES, .. NOT_FOUND_SUMS];
}











namespace TextData
{
    struct AppConstants        
    {
        

        
                
        public static Dictionary<string, string> drive_names = new()      // print out
        {
            [drive_tags[0]] = "ИСХОДНАЯ",
            [drive_tags[1]] = "РЕЗЕРВНАЯ",
            [drive_tags[2]] = "ОТЧЕТНАЯ"
        };

        

        

        

        
        
        
        

        public const char line = '-';

        
    }


    




        
    class PrintFullLog
    {
        public Dictionary<string, int>? All_Sums { get; set; }
        public Dictionary<string, int>? Simple_Sums { get; set; }

        private readonly Action<string> AllSumsLineLogOut; 
        private readonly Action<string> SimpleSumsLineLogOut; 

        public PrintFullLog(Dictionary<string, int> all_sums, Dictionary<string, int>? simple_sums)
        {
            All_Sums = all_sums;
            Simple_Sums = simple_sums;

            AllSumsLineLogOut = (sum_name) => Console.WriteLine($"> {sum_name}: {All_Sums[sum_name]}");
            SimpleSumsLineLogOut = (sum_name) => Console.WriteLine($"> {sum_name}: {Simple_Sums![sum_name]}");
        }

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

            if (Simple_Sums is not null)
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



    class AppInfoConsoleOut           // можно классифицировать и разделить
    {
        private const char star = '*';
        private const char grille = '#';

        private static readonly string star_line = new(star, 45);
                
        public static void ShowLine()
        {
            Console.WriteLine(new string(AppConstants.line, 45));
        }

        public static void ShowStarLine()
        {
            Console.WriteLine(star_line);
        }

        public static void ShowProgramInfo()
        {
            Console.WriteLine($"{star_line}\n\n >> Backup PDF v.2.0 <<\n\n{star_line}");
        }

        public static void ShowEnterPeriod()
        {
            List<string> value_info = [];

            foreach (var month in AppConstants.month_names)
            {
                value_info.Add(string.Concat(month, AppConstants.line, $"\"{AppConstants.month_names.IndexOf(month) + 1}\""));
            }

            value_info.Add(string.Concat(AppConstants.year, AppConstants.line, $"\"{CurrentDate.Year}\""));

            Console.WriteLine(">>> Введите значение периода, за который выполнить копирование >>>\n");
            Console.WriteLine($" | {string.Join("; ", value_info)} |");

            Console.WriteLine('\n');
            ShowLine();
        }

        public static void ShowNullTextError()
        {
            Console.WriteLine($" {grille} Вы ничего не ввели, вводите заново !");
        }
        
        public static void ShowDirectorySetupTrue(string drive_type, string directory)
        {
            Console.WriteLine($" {star} {AppConstants.drive_names[drive_type]} директория: {directory}");    
        }

        public static void ShowDirectoryExistFalse(string drive_type)
        {
            Console.WriteLine($" {grille} {AppConstants.drive_names[drive_type]} директория не существует, установите правильную !");
        }

        public static void ShowInstallDirectory(string drive_type)
        {
            Console.WriteLine($" {grille} {AppConstants.drive_names[drive_type]} директория успешно установлена !");
        }

        public static void ShowScansNotFound(string period)
        {
            Console.WriteLine($" {grille} За {period} сканов не найдено !");
        }

        public static void ShowLogHeader(string period)
        {
            string borders = new(star, 3);
            Console.WriteLine($" {borders} Отчет за {period} {borders}");
        }

        public static void ShowResult()
        {
            Console.WriteLine($" {grille} Резервное копирование завершено успешно !");
        }

        public static void ShowMonthBackupResult(string period, int file_count)
        {
            Console.WriteLine($" {star} За {period} успешно скопировано _{file_count}_ файл(ов) !");
        }
    }


    class InputNoNullText
    {
        public static string GetRealText()
        {
            string? text;
            bool real_text;

            do
            {
                text = Console.ReadLine();
                real_text = string.IsNullOrEmpty(text);

                if (real_text)
                {
                    AppInfoConsoleOut.ShowNullTextError();
                }
            } while (real_text);

            return text!;
        }
    }
}
