// * Файл "FullLogPrinter.cs": класс для вывода отчета. *

class FullLogPrinter
{
    // Вывод общих сумм в одну строку по имени (ключу).

    readonly Action<string> MainSumsLineLogOut;

    // Вывод сумм простых протоколов в одну строку по ключу.

    readonly Action<string> SimpleSumsLineLogOut;

    // Словарь общих сумм.

    public Dictionary<string, int>? Main_Protocol_Sums_in { get; set; }

    // Словарь сумм простых протоколов.

    public Dictionary<string, int>? Simple_Protocol_Sums_in { get; set; }

    // Начальные параметры: общие суммы и суммы простых протоколов.

    public FullLogPrinter(Dictionary<string, int> main_sums, Dictionary<string, int>? simple_sums)
    {
        Main_Protocol_Sums_in = main_sums;
        Simple_Protocol_Sums_in = simple_sums;

        MainSumsLineLogOut = (sum_name) => Console.WriteLine($"> {sum_name}: {Main_Protocol_Sums_in[sum_name]}");
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

        MainSumsLineLogOut(ProtocolTypesAndSums.MAIN_SUMS[0]);

        // Сумма протоколов ЕИАС.

        MainSumsLineLogOut(ProtocolTypesAndSums.MAIN_SUMS[1]);

        SeparateString();

        // Сумма протоколов по ФФ.

        MainSumsLineLogOut(ProtocolTypesAndSums.MAIN_SUMS[2]);

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
