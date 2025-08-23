namespace TextData
{
    struct AppConstants
    {
        public const string drives_config_file = "C:\\Users\\Mahabhara\\source\\repos\\Fluenoriph\\Backup-Scans\\drives_config.xml"; // !! относит.
        
        public const string month_logs_file = "backup_log_monthes.xml";
        public const string year_log_file = "year_log.xml";

        public static List<string> drive_type = ["SOURCE", "DESTINATION"];
        public static Dictionary<string, string> drive_names = new()
        {
            [drive_type[0]] = "ИСХОДНАЯ",
            [drive_type[1]] = "РЕЗЕРВНАЯ"
        };

        public const string number_group = "^(?<number>";
        public static string eias_number_pattern = string.Concat(number_group, "\\d{5}-\\d{2}-\\d{2})-");
        public static string simple_number_pattern = string.Concat(number_group, "\\d{1,4})-");
        
        public const string protocol_file_type = "pdf";

        public const int january_index = 0;
        public const int october_index = 9;
        public const int december_index = 11;

        public const string year = "Год";
        public static List<string> month_names = ["Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"];

        public static List<string> types_full_names = ["Физические факторы (Уссурийск)", "Физические факторы (Арсеньев)",
                                                           "Радиационный контроль (Уссурийск)", "Радиационный контроль (Арсеньев)",
                                                           "Измерения мебели (Уссурийск)", "Измерения мебели (Арсеньев)"];
        
        public static List<string> types_short_names = ["ф", "фа", "р", "ра", "м", "ма"];
        
        public const string sums_tag = "sums";
        public const string names_tag = "protocol_names";

        public static List<string> others_sums = ["Всего", "ЕИАС", "Простые"];
        public static List<string> others_sums_tags = ["full", "eias", "simple"];

        public static List<string> full_location_sums = ["Уссурийск всего", "Арсеньев всего"];
        
        public static List<string> full_type_sums = ["Физические факторы всего", "Радиационный контроль всего", "Измерения мебели всего"];
        
        public static List<string> not_found_sums = ["Пропущенные", "Неизвестные"];
        
        public static List<string> simple_sums_tags = ["uss", "ars", "f_all", "r_all", "m_all", "f", "fa", "r", "ra", "m", "ma", "misseds", "unknowns"];
        public static List<string> type_tags = simple_sums_tags.GetRange(5, 6);

        public const char line = '-';

        public static List<string> united_type_names = [.. full_location_sums, .. full_type_sums, .. types_full_names, .. not_found_sums];
    }


    struct CurrentDate
    {
        public static int Year   // day....
        {
            get
            {
                DateTime current_date = DateTime.Now;
                return current_date.Year;
            }
        }
    }

        
    class ConsoleOutFullLog(Dictionary<string, int> all_sums, Dictionary<string, int>? simple_sums)
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

            Console.WriteLine(" Введите значение периода, за который выполнить копирование:\n");
            Console.WriteLine($" ({string.Join("; ", value_info)})");
            ShowLine();
        }

        public static void ShowNullTextError()
        {
            Console.WriteLine($" {grille} Вы ничего не ввели, вводите заново !");
        }
        
        public static void ShowDirectorySetupTrue(string name, string directory)
        {
            Console.WriteLine($" {star} {AppConstants.drive_names[name]} директория: {directory}");
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
