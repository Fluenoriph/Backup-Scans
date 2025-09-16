/* 
 * * Файл "ProgramInfoConsoleOut.cs": консольный "интерфейс" программы.
 * 
 *   Содержит следующие пространства имен:
 *   
 *   1. "InfoOut": вывод различной информации;
 *   2. "InputValidate": проверка ввода;
 *   3. "ResultLogOut": вывод полного отчета.
 */

namespace InfoOut
{
    /*
     * 1. "BackupInfo": класс для вывода информации, связанной с резервным копированием;
     * 2. "GeneralInfo": класс для вывода основной информации;
     * 3. "ParameterTemplates": класс шаблон вывода параметров выбора;
     * 4. "WorkDirectoriesInfo": класс для вывода информации, связанной с рабочими директориями.
     */

    class BackupInfo
    {
        // * Сообщение о том, что протоколы не найдены. *

        public static void ShowScansNotFound(string period)
        {
            Console.WriteLine($"\n {Symbols.GRILLE} За {period} сканов не найдено !");
        }

        // * Заголовок отчета. *

        public static void ShowLogHeader(string period)
        {
            string borders_lcl = new(Symbols.STAR, 3);
            Console.WriteLine($" {borders_lcl} Отчет за {period} {borders_lcl}");
        }

        // * Положительный результат. *

        public static void ShowResult()
        {
            Console.WriteLine($"\n {Symbols.GRILLE} Резервное копирование завершено успешно !\n");
        }

        // * Вывод результата количества скопированных файлов за месяц. *

        public static void ShowMonthBackupResult(string period, int file_count)
        {
            Console.WriteLine($" {Symbols.STAR} За {period} успешно скопировано < {file_count} > файл(ов)");
        }

        // * Сообщение об ошибке копирования. *

        public static void ShowCopyError()
        {
            Console.WriteLine($" {Symbols.GRILLE} Критическая ошибка копирования файлов !\n   Перезапустите программу !");
        }

        // * Сообщение об ожидании. *

        public static void ShowVisualWait()
        {
            Console.Write($"\n {Symbols.GRILLE} Выполняется копирование. Ожидайте ...\n\n");
        }
    }

        
    class GeneralInfo
    {
        static readonly string star_line = new(Symbols.STAR, 60);

        // * Показать линию (-----). *

        public static void ShowLine()
        {
            Console.WriteLine(new string(Symbols.LINE, 60));
        }

        // * Показать линию из звездочек (*******). *

        public static void ShowStarLine()
        {
            Console.WriteLine(star_line);
        }

        // * Информация об авторе программы. *

        public static void ShowAuthorInfo()
        {
            Console.WriteLine("\n [ Иван Богданов. Все права защищены. 2025 г. ]\n");
        }

        // * Показать информацию о программе. *

        public static void ShowProgramInfo()
        {
            Console.WriteLine("\n >> Backup \"PDF\" Protocols Scan Files v.2.0 <<\n");
        }

        // * Показать главное меню программы. *

        public static void ShowProgramMenu()
        {
            List<string> value_info_lcl = [];

            // Создание списка параметров по периодам.

            foreach (var month in PeriodsNames.MONTHES)
            {
                value_info_lcl.Add(string.Concat(month, Symbols.LINE, ParameterTemplates.CreateParameterDigit(PeriodsNames.MONTHES, month)));
            }
            value_info_lcl.Add(string.Concat(PeriodsNames.YEAR, Symbols.LINE, $"\"{CurrentDate.Year_in}\""));

            // Вывод.

            Console.WriteLine($" {Symbols.STAR} МЕНЮ {Symbols.STAR}\n");
                        
            Console.WriteLine($"{Symbols.FLOW_RIGHT} Для запуска резервного копирования, введите значение периода {Symbols.FLOW_RIGHT}\n");
            ParameterTemplates.ShowParameters(value_info_lcl);
            Console.WriteLine('\n');

            Console.WriteLine($"{Symbols.FLOW_RIGHT} Чтобы изменить директорию, введите: \"{Symbols.CHANGE_DIRECTORY_FUNCTION}\"\n");
            ShowLine();
        }

        // * Сообщение о рестарте или завершении программы. *

        public static bool RestartOrExitProgram()
        {
            Console.WriteLine($"\n\n {new string('/', 3)} Для выхода в главное меню нажмите < ПРОБЕЛ >, чтобы завершить работу программы нажмите любую клавишу {new string('\\', 3)}");

            if (Console.ReadKey(intercept: true).Key is ConsoleKey.Spacebar)
            {
                Console.Clear();

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    
    class ParameterTemplates
    {
        // * Вывод списка параметров "out_info". *

        public static void ShowParameters(List<string> out_info)
        {
            Console.WriteLine($" | {string.Join("; ", out_info)} |");
        }

        // * Создание параметра цифры по индексу значения.
        //   Параметры: список из которого получить индекс, значение параметра.

        public static string CreateParameterDigit(List<string> parameters_type, string indexing_value)
        {
            return $"\"{parameters_type.IndexOf(indexing_value) + 1}\"";
        }
    }
           

    class WorkDirectoriesInfo
    {
        // Кириллические названия "дисков". 

        struct WorkDirectories
        {
            public static Dictionary<string, string> NAMES = new()
            {
                [XmlTags.DRIVE_TAGS[0]] = "ИСХОДНАЯ",
                [XmlTags.DRIVE_TAGS[1]] = "РЕЗЕРВНАЯ",
                [XmlTags.DRIVE_TAGS[2]] = "ОТЧЕТНАЯ"
            };
        }

        const string DIR_EXAMPLE = "Пример: C:\\Folder\\Subfolder";

        // * Сообщение о положительной установке директории. *

        public static void ShowDirectorySetupTrue(string drive_type, string directory)
        {
            Console.WriteLine($" {Symbols.STAR} {WorkDirectories.NAMES[drive_type]} директория: {directory}\n");
        }

        // * Сообщение об отрицательной установке директории. *

        public static void ShowDirectoryExistFalse(string drive_type, string directory)
        {
            if (directory is "")
            {
                Console.WriteLine($" {Symbols.GRILLE} {WorkDirectories.NAMES[drive_type]} директория не установлена, необходимо установить.\n");
            }
            else
            {
                Console.WriteLine($" {Symbols.GRILLE} {WorkDirectories.NAMES[drive_type]} директория [{directory}] не существует, установите правильную !\n");
            }
        }

        // * Сообщение о том, что директория установлена. *

        public static void ShowInstallDirectory(string drive_type)
        {
            Console.WriteLine($"\n {Symbols.GRILLE} {WorkDirectories.NAMES[drive_type]} директория успешно установлена !");
        }

        // * Сообщение о смене директории. *

        public static void ShowEnterDirectoryType()
        {
            List<string> dir_type_info_lcl = [];

            // Создание параметров для выбора.

            foreach (var drive in XmlTags.DRIVE_TAGS)
            {
                dir_type_info_lcl.Add(string.Concat(ParameterTemplates.CreateParameterDigit(XmlTags.DRIVE_TAGS, drive), Symbols.LINE, WorkDirectories.NAMES[drive]));
            }

            // Сам вывод.

            Console.WriteLine($" {Symbols.STAR} Выберите тип директории {Symbols.FLOW_RIGHT}\n");
            ParameterTemplates.ShowParameters(dir_type_info_lcl);
            GeneralInfo.ShowLine();
        }

        // * Сообщение о вводе директории. *

        public static void ShowEnterTheDirectory()
        {
            Console.WriteLine($"\n{Symbols.FLOW_RIGHT} Введите директорию:\n\n {DIR_EXAMPLE}");
        }
    }
}


namespace InputValidate
{
    /*
     * 1. "DriveIndex": класс для получения индекса "диска" из введенных символов. Только 1, 2 или 3;
     * 2. "InputNoNullText": класс для проверки ввода пустой строки.
     */

    class DriveIndex
    {
        public static int Index_in
        {
            get
            {
                var input_symbol = Console.ReadKey(intercept: true).Key;

                if (input_symbol is ConsoleKey.D1 or ConsoleKey.NumPad1)
                {
                    return 0;
                }
                else if (input_symbol is ConsoleKey.D2 or ConsoleKey.NumPad2)
                {
                    return 1;
                }
                else if (input_symbol is ConsoleKey.D3 or ConsoleKey.NumPad3)
                {
                    return 2;
                }
                else
                {
                    return -1;
                }
            }
        }
    }


    class InputNoNullText
    {
        // * Принудительно получить правильное значение. *

        public static string GetRealText()
        {
            string? text_lcl;
            bool real_text_status;

            // Цикл повторяется, до тех пор, пока не введут реальное значение.

            do
            {
                text_lcl = Console.ReadLine();
                real_text_status = string.IsNullOrEmpty(text_lcl);

                if (real_text_status)
                {
                    Console.WriteLine($" {Symbols.GRILLE} Вы ничего не ввели, вводите заново !\n");
                }
            } while (real_text_status);

            return text_lcl!;
        }
    }
}


namespace ResultLogOut
{
    // 1. "FullLogPrinter": класс для вывода лога.

    class FullLogPrinter
    {
        // Вывод общих сумм в одну строку по имени (ключу).

        readonly Action<string> AllSumsLineLogOut;

        // Вывод сумм простых протоколов в одну строку по ключу.

        readonly Action<string> SimpleSumsLineLogOut;

        // Словарь общих сумм.

        public Dictionary<string, int>? All_Protocol_Sums_in { get; set; }

        // Словарь сумм простых протоколов.

        public Dictionary<string, int>? Simple_Protocol_Sums_in { get; set; }

        // Начальные параметры: общие суммы и суммы простых протоколов.

        public FullLogPrinter(Dictionary<string, int> all_sums, Dictionary<string, int>? simple_sums)
        {
            All_Protocol_Sums_in = all_sums;
            Simple_Protocol_Sums_in = simple_sums;

            AllSumsLineLogOut = (sum_name) => Console.WriteLine($"> {sum_name}: {All_Protocol_Sums_in[sum_name]}");
            SimpleSumsLineLogOut = (sum_name) => Console.WriteLine($"> {sum_name}: {Simple_Protocol_Sums_in![sum_name]}");
        }

        // * Вывод отчета. *

        public void ShowLog()
        {
            // * Разделитель строк в консоли. *

            static void SeparateString()
            {
                Console.WriteLine(new string('=', 60));
            }

            // Вывод общих сумм.

            SeparateString();

            // Все протоколы.

            AllSumsLineLogOut(ProtocolTypesAndSums.MAIN_SUMS[0]);

            // Сумма протоколов ЕИАС.

            AllSumsLineLogOut(ProtocolTypesAndSums.MAIN_SUMS[1]);

            SeparateString();

            // Сумма протоколов по ФФ.

            AllSumsLineLogOut(ProtocolTypesAndSums.MAIN_SUMS[2]);

            // Вывод сумм типов протоколов по ФФ, если они есть.

            if (Simple_Protocol_Sums_in is not null)
            {
                // Суммы всех протоколов по локациям.

                ProtocolTypesAndSums.FULL_LOCATION_SUMS.ForEach(name => SimpleSumsLineLogOut(name));
                                
                SeparateString();

                // Физические факторы.

                // Всего.
                // Уссурийск.
                // Арсеньев.

                SimpleSumsLineLogOut(ProtocolTypesAndSums.FULL_TYPE_SUMS[0]);
                SimpleSumsLineLogOut(ProtocolTypesAndSums.TYPES_FULL_NAMES[0]);
                SimpleSumsLineLogOut(ProtocolTypesAndSums.TYPES_FULL_NAMES[1]);
                                
                SeparateString();

                // Радиационный контроль.

                // Всего.
                // Уссурийск.
                // Арсеньев.

                SimpleSumsLineLogOut(ProtocolTypesAndSums.FULL_TYPE_SUMS[1]);
                SimpleSumsLineLogOut(ProtocolTypesAndSums.TYPES_FULL_NAMES[2]);
                SimpleSumsLineLogOut(ProtocolTypesAndSums.TYPES_FULL_NAMES[3]);
                                
                SeparateString();

                // Измерения мебели.

                // Всего.
                // Уссурийск.
                // Арсеньев.

                SimpleSumsLineLogOut(ProtocolTypesAndSums.FULL_TYPE_SUMS[2]);
                SimpleSumsLineLogOut(ProtocolTypesAndSums.TYPES_FULL_NAMES[4]);
                SimpleSumsLineLogOut(ProtocolTypesAndSums.TYPES_FULL_NAMES[5]);

                // Пропущенные и неизвестные.

                SeparateString();

                ProtocolTypesAndSums.NOT_FOUND_SUMS.ForEach(name => SimpleSumsLineLogOut(name));
            }

            SeparateString();
        }
    }
}
