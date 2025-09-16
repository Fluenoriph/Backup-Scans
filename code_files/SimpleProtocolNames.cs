// * Файл SimpleProtocolNames.cs: класс для вычислений имен простых протоколов (физ. факторов) за месяц. *

// Это класс ProtocolNamesComputing для вычисления имен всех протоколов. Отдельная задача.

class SimpleProtocolNames
{
    readonly SimpleConvert self_obj_number_converter_in = new();

    // Функция для получение сокращенного обозначения типа протокола по ключу.

    readonly Func<string, string> GetShortTypeName = (key) => ProtocolTypesAndSums.TYPES_SHORT_NAMES[ProtocolTypesAndSums.TYPES_FULL_NAMES.IndexOf(key)];
    
    // Коллекция минимальных номеров протоколов за текущий, выбранный месяц. Главное использование этого поля, это вычисление неизвестных (unknown) протоколов.

    readonly Dictionary<string, int> current_period_min_numbers_in;
        
    // Коллекция сортированных численных номеров протоколов каждого найденного типа (типа физ. факторов).

    readonly Dictionary<string, List<int>> protocol_numeric_numbers_in;

    // Словарь сортированных по возрастанию имен протоколов каждого типа.

    public Dictionary<string, List<string>> Sorted_Names_in { get; } = [];

    // Список пропущенных протоколов по возрастанию номера, каждого типа, все вместе. Только номер и тип.

    public List<string>? Missed_Protocols_in { get; set; }

    // Список неизвестных протоколов по возрастанию номера, все типы вместе. Номер и тип.

    public List<string>? Unknown_Protocols_in { get; set; }

    // Входной параметр: словарь простых протоколов.

    public SimpleProtocolNames(Dictionary<string, List<FileInfo>> files)
    {
        protocol_numeric_numbers_in = GetSortedProtocolNumbers(files);

        MinimumNumbers self_obj_extreme_min_lcl = new(protocol_numeric_numbers_in);

        // Минимальные номера вычисляются заранее.

        current_period_min_numbers_in = self_obj_extreme_min_lcl.Numbers_in;

        SimpleSort self_obj_sorter_lcl = new();

        foreach (var item in files)
        {
            Sorted_Names_in.Add(item.Key, self_obj_sorter_lcl.Sorting(protocol_numeric_numbers_in[item.Key], item.Value));
        }

        ComputeMissedProtocols();
    }

    // * Преобразование словаря файлов протоколов, в словарь сих сортированных номеров. *

    Dictionary<string, List<int>> GetSortedProtocolNumbers(Dictionary<string, List<FileInfo>> files)
    {
        Dictionary<string, List<int>> numbers_lcl = [];

        foreach (var item in files)
        {
            numbers_lcl.Add(item.Key, self_obj_number_converter_in.ConvertToNumbers(item.Value));
        }

        return numbers_lcl;
    }

    // * Вычисление пропущенных протоколов. *

    void ComputeMissedProtocols()
    {
        MaximumNumbers self_obj_extreme_max_lcl = new(protocol_numeric_numbers_in);
        var max_numbers_lcl = self_obj_extreme_max_lcl.Numbers_in;

        // Список для проверки в конце вычисления, есть ли пропущенные.

        List<string> missed_protocols_lcl = [];

        // Алгоритм производит вычисления с коллекцией номеров протоколов.

        foreach (var item in protocol_numeric_numbers_in)
        {
            // Если в списке типа протокола два или больше протокола, то можно искать пропущенные.
            // # Например: в списке два протокола с номерами 10 и 14. Значит 3 пропущены (11, 12, 13).

            if (item.Value.Count >= 2)
            {
                // Создаем диапазон всех номеров. Предположительно, в нем пропущенные.

                var range_lcl = Enumerable.Range(current_period_min_numbers_in[item.Key] + 1, max_numbers_lcl[item.Key] - current_period_min_numbers_in[item.Key]);

                // Получение разности множеств. Если она есть, то это пропущенные.

                IEnumerable<int> missed_lcl = range_lcl.Except(item.Value);

                if (missed_lcl.Any())      
                {
                    List<int> missed_numbers_lcl = [.. missed_lcl];

                    foreach (int number in missed_numbers_lcl)
                    {
                        missed_protocols_lcl.Add($"{number}-{GetShortTypeName(item.Key)}");
                    }
                }
            }
        }

        if (missed_protocols_lcl.Count != 0)
        {
            Missed_Protocols_in = missed_protocols_lcl;
        }
        else
        {
            Missed_Protocols_in = null;
        }
    }

    // * Вычисление неизвестных протоколов. *

    // Параметр: "previous_period_files" - найденные файлы за предыдущий месяц.

    public void ComputeUnknownProtocols(Dictionary<string, List<FileInfo>> previous_period_files)
    {
        MaximumNumbers self_obj_previous_extreme_max_lcl = new(GetSortedProtocolNumbers(previous_period_files));
        var max_numbers_lcl = self_obj_previous_extreme_max_lcl.Numbers_in;

        List<string> unknown_protocols_lcl = [];

        // Алгоритм сопоставляет минимальные номера текущего месяца, с максимальными номерами предыдущего

        /* # Пропущенные будут тогда, когда есть разрыв в нумерации за два подряд идущих месяца в годовом порядке.
             Пример: за январь есть скан № 23-ф, а за февраль № 26-ф. Следовательно, два протокола неизвестные. Или за январь, или за февраль. */

        foreach (var item in current_period_min_numbers_in!)
        {
            // Поиск неизвестных будет работать, если в предыдущем месяце есть такой же тип протоколов, как и в текущем.

            // "value", максимальный номер предыдущего месяца - это текущий минимальный номер для вычисления.

            if (max_numbers_lcl.TryGetValue(item.Key, out int value))
            {
                int max_number_lcl = item.Value;

                // Подтверждающий статус, что есть пропущенные. Если есть разрыв в порядковых номерах.

                bool unknowns_found_status = (value < max_number_lcl) && ((max_number_lcl - 1) != value);

                // Добавление этих номеров.

                if (unknowns_found_status)
                {
                    for (int start_num = value + 1; start_num < max_number_lcl; start_num++)
                    {
                        unknown_protocols_lcl.Add($"{start_num}{Symbols.LINE}{GetShortTypeName(item.Key)}");
                    }
                }
            }      
        }

        if (unknown_protocols_lcl.Count != 0)
        {
            Unknown_Protocols_in = unknown_protocols_lcl;
        }
        else
        {
            Unknown_Protocols_in = null;
        }
    }
}
