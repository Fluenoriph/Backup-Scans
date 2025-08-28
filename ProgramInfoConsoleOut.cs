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


class AnyInfo
{
    private static readonly string star_line = new(Symbols.STAR, 45);

    public static void ShowLine()
    {
        Console.WriteLine(new string(Symbols.LINE, 45));
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
        List<string> value_info_lcl = [];

        foreach (var month in PeriodsNames.MONTHES)
        {
            value_info_lcl.Add(string.Concat(month, Symbols.LINE, $"\"{PeriodsNames.MONTHES.IndexOf(month) + 1}\""));
        }

        value_info_lcl.Add(string.Concat(PeriodsNames.YEAR, Symbols.LINE, $"\"{CurrentDate.Year}\""));

        Console.WriteLine(">>> Введите значение периода, за который выполнить копирование >>>\n");
        Console.WriteLine($" | {string.Join("; ", value_info_lcl)} |");

        Console.WriteLine('\n');
        ShowLine();
    }
}


class DirectoriesInfo
{
    private const string DIR_in = "директория";

    public static void ShowDirectorySetupTrue(string drive_type, string directory)
    {
        Console.WriteLine($" {Symbols.STAR} {WorkDirectories.NAMES[drive_type]} {DIR_in}: {directory}");
    }

    public static void ShowDirectoryExistFalse(string drive_type)
    {
        Console.WriteLine($" {Symbols.GRILLE} {WorkDirectories.NAMES[drive_type]} {DIR_in} не существует, установите правильную !");
    }

    public static void ShowInstallDirectory(string drive_type)
    {
        Console.WriteLine($" {Symbols.GRILLE} {WorkDirectories.NAMES[drive_type]} {DIR_in} успешно установлена !");
    }
}


class BackupInfo
{
    public static void ShowScansNotFound(string period)
    {
        Console.WriteLine($" {Symbols.GRILLE} За {period} сканов не найдено !");
    }

    public static void ShowLogHeader(string period)
    {
        string borders_lcl = new(Symbols.STAR, 3);
        Console.WriteLine($" {borders_lcl} Отчет за {period} {borders_lcl}");
    }

    public static void ShowResult()
    {
        Console.WriteLine($" {Symbols.GRILLE} Резервное копирование завершено успешно !");
    }

    public static void ShowMonthBackupResult(string period, int file_count)
    {
        Console.WriteLine($" {Symbols.STAR} За {period} успешно скопировано _{file_count}_ файл(ов) !");
    }

    public static void ShowCopyError()
    {
        Console.WriteLine($" {Symbols.GRILLE} Критическая ошибка ! Копирование остановлено.\n   Перезапустите программу !");
    }
}


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
            Console.WriteLine(new string('=', 45));
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
