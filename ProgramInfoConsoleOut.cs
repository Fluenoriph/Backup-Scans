namespace InputValidate
{
    class InputNoNullText
    {
        public static string GetRealText()
        {
            string? text_lcl;
            bool real_text_status;

            do
            {
                text_lcl = Console.ReadLine();
                real_text_status = string.IsNullOrEmpty(text_lcl);

                if (real_text_status)
                {
                    Console.WriteLine($" {Symbols.GRILLE} Вы ничего не ввели, вводите заново !");
                }
            } while (real_text_status);

            return text_lcl!;
        }
    }


    class DriveIndex
    {
        public static int Index_in { get; }

        static DriveIndex()
        {
            var input_symbol = Console.ReadKey(intercept: true).Key;

            if (input_symbol is (ConsoleKey.D1 or ConsoleKey.NumPad1))
            {
                Index_in = 0;
            }
            else if (input_symbol is (ConsoleKey.D2 or ConsoleKey.NumPad2))
            {
                Index_in = 1;
            }
            else if (input_symbol is (ConsoleKey.D3 or ConsoleKey.NumPad3))
            {
                Index_in = 2;
            }
            else
            {
                _ = new ProgramShutDown(ErrorCodes.INPUT_VALUE_ERROR);
            }
        }
    }
}


namespace InfoOut
{
    class ParameterTemplates
    {
        public static void ShowParameters(List<string> out_info)
        {
            Console.WriteLine($" | {string.Join("; ", out_info)} |");
        }

        public static string CreateParameterDigit(List<string> parameters_type, string indexing_value)
        {
            return $"\"{parameters_type.IndexOf(indexing_value) + 1}\"";
        }
    }


    class GeneralInfo
    {
        private static readonly string star_line = new(Symbols.STAR, 60);

        public static void ShowLine()
        {
            Console.WriteLine(new string(Symbols.LINE, 60));
        }

        public static void ShowStarLine()
        {
            Console.WriteLine(star_line);
        }

        public static void ShowProgramInfo()
        {
            Console.WriteLine($"{star_line}\n\n >> Backup PDF v.2.0 <<\n\n{star_line}\n");
        }

        public static void ShowProgramMenu()
        {
            List<string> value_info_lcl = [];

            foreach (var month in PeriodsNames.MONTHES)
            {
                value_info_lcl.Add(string.Concat(month, Symbols.LINE, ParameterTemplates.CreateParameterDigit(PeriodsNames.MONTHES, month)));
            }

            value_info_lcl.Add(string.Concat(PeriodsNames.YEAR, Symbols.LINE, $"\"{CurrentDate.Year}\""));

            Console.WriteLine($" {Symbols.STAR} МЕНЮ\n");

            Console.WriteLine($"{Symbols.FLOW_RIGHT} Чтобы изменить директорию, введите: \"{Symbols.CHANGE_DIRECTORY_FUNCTION}\"\n");
            Console.WriteLine($"{Symbols.FLOW_RIGHT} Для запуска копирования, введите значение периода {Symbols.FLOW_RIGHT}\n");
            ParameterTemplates.ShowParameters(value_info_lcl);

            Console.WriteLine('\n');
            ShowLine();
        }

        public static bool RestartOrExitProgram()
        {
            Console.WriteLine($"\n\n {new string('/', 3)} Для выхода в главное меню нажмите < ПРОБЕЛ >, чтобы завершить работу программы нажмите любую клавишу {new string('\\', 3)}");

            if (Console.ReadKey(intercept: true).Key is ConsoleKey.Spacebar)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    class WorkDirectoriesInfo
    {
        private const string DIR_EXAMPLE = "Пример: C:\\Folder\\Subfolder";

        public static void ShowDirectorySetupTrue(string drive_type, string directory)
        {
            Console.WriteLine($" {Symbols.STAR} {WorkDirectories.NAMES[drive_type]} директория: {directory}");
        }

        public static void ShowDirectoryExistFalse(string drive_type, string directory)
        {
            if (directory is "")
            {
                Console.WriteLine($" {Symbols.GRILLE} {WorkDirectories.NAMES[drive_type]} директория не установлена, введите директорию {Symbols.FLOW_RIGHT}\n\n   {DIR_EXAMPLE}");
            }
            else
            {
                Console.WriteLine($" {Symbols.GRILLE} {WorkDirectories.NAMES[drive_type]} директория [{directory}] не существует, установите правильную !\n\n   {DIR_EXAMPLE}");
            }
        }

        public static void ShowInstallDirectory(string drive_type)
        {
            Console.WriteLine($" {Symbols.GRILLE} {WorkDirectories.NAMES[drive_type]} директория успешно установлена !");
        }

        public static void ShowEnterDirectoryType()
        {
            List<string> dir_type_info_lcl = [];

            foreach (var drive in XmlTags.DRIVE_TAGS)
            {
                dir_type_info_lcl.Add(string.Concat(ParameterTemplates.CreateParameterDigit(XmlTags.DRIVE_TAGS, drive), Symbols.LINE, WorkDirectories.NAMES[drive]));
            }

            Console.WriteLine($" {Symbols.STAR} Выберите тип директории {Symbols.FLOW_RIGHT}\n");
            ParameterTemplates.ShowParameters(dir_type_info_lcl);
            GeneralInfo.ShowLine();
        }

        public static void ShowEnterTheDirectory()
        {
            Console.WriteLine($"\n{Symbols.FLOW_RIGHT} Введите директорию:\n\n {DIR_EXAMPLE}");
        }

        public static void ShowResourceDenied(string drive_type)
        {
            Console.WriteLine($"\n {Symbols.GRILLE} Доступ к ресурсу '{drive_type}' запрещен ! Перезапустите программу или измените путь\n");
        }
    }


    class BackupInfo
    {
        public static void ShowScansNotFound(string period)
        {
            Console.WriteLine($"\n {Symbols.GRILLE} За {period} сканов не найдено !");
        }

        public static void ShowLogHeader(string period)
        {
            string borders_lcl = new(Symbols.STAR, 3);
            Console.WriteLine($" {borders_lcl} Отчет за {period} {borders_lcl}");
        }

        public static void ShowResult()
        {
            Console.WriteLine($"\n {Symbols.GRILLE} Резервное копирование завершено успешно !\n");
        }

        public static void ShowMonthBackupResult(string period, int file_count)
        {
            Console.WriteLine($" {Symbols.STAR} За {period} успешно скопировано _{file_count}_ файл(ов)");
        }

        public static void ShowCopyError()
        {
            Console.WriteLine($" {Symbols.GRILLE} Критическая ошибка копирования файлов !\n   Перезапустите программу !");
        }

        public static void ShowVisualWait()
        {
            Console.Write($"\n {Symbols.GRILLE} Выполняется копирование. Ожидайте ...\n\n");
        }
    }
}


namespace ResultLogOut
{
    class FullLogPrinter
    {
        private readonly Action<string> AllSumsLineLogOut;
        private readonly Action<string> SimpleSumsLineLogOut;

        public Dictionary<string, int>? All_Protocol_Sums_in { get; set; }
        public Dictionary<string, int>? Simple_Protocol_Sums_in { get; set; }

        public FullLogPrinter(Dictionary<string, int> all_sums, Dictionary<string, int>? simple_sums)
        {
            All_Protocol_Sums_in = all_sums;
            Simple_Protocol_Sums_in = simple_sums;

            AllSumsLineLogOut = (sum_name) => Console.WriteLine($"> {sum_name}: {All_Protocol_Sums_in[sum_name]}");
            SimpleSumsLineLogOut = (sum_name) => Console.WriteLine($"> {sum_name}: {Simple_Protocol_Sums_in![sum_name]}");
        }

        public void ShowLog()
        {
            static void SeparateString()
            {
                Console.WriteLine(new string('=', 60));
            }

            SeparateString();

            AllSumsLineLogOut(ProtocolTypesSums.OTHERS_SUMS[0]);
            AllSumsLineLogOut(ProtocolTypesSums.OTHERS_SUMS[1]);

            SeparateString();

            AllSumsLineLogOut(ProtocolTypesSums.OTHERS_SUMS[2]);

            if (Simple_Protocol_Sums_in is not null)
            {
                ProtocolTypesSums.FULL_LOCATION_SUMS.ForEach(name => SimpleSumsLineLogOut(name));

                SeparateString();

                SimpleSumsLineLogOut(ProtocolTypesSums.FULL_TYPE_SUMS[0]);
                SimpleSumsLineLogOut(ProtocolTypesSums.TYPES_FULL_NAMES[0]);
                SimpleSumsLineLogOut(ProtocolTypesSums.TYPES_FULL_NAMES[1]);

                SeparateString();

                SimpleSumsLineLogOut(ProtocolTypesSums.FULL_TYPE_SUMS[1]);
                SimpleSumsLineLogOut(ProtocolTypesSums.TYPES_FULL_NAMES[2]);
                SimpleSumsLineLogOut(ProtocolTypesSums.TYPES_FULL_NAMES[3]);

                SeparateString();

                SimpleSumsLineLogOut(ProtocolTypesSums.FULL_TYPE_SUMS[2]);
                SimpleSumsLineLogOut(ProtocolTypesSums.TYPES_FULL_NAMES[4]);
                SimpleSumsLineLogOut(ProtocolTypesSums.TYPES_FULL_NAMES[5]);

                SeparateString();

                ProtocolTypesSums.NOT_FOUND_SUMS.ForEach(name => SimpleSumsLineLogOut(name));
            }

            SeparateString();
        }
    }
}
