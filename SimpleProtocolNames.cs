class SimpleProtocolNames
{
    private readonly Func<string, string> GetShortTypeName = (key) => ProtocolTypesSums.TYPES_SHORT_NAMES[ProtocolTypesSums.TYPES_FULL_NAMES.IndexOf(key)];
    private readonly Dictionary<string, int> current_period_min_numbers_in;
    private readonly SimpleConvert number_converter_in = new();
    private readonly Dictionary<string, List<int>> numeric_numbers_in;

    public Dictionary<string, List<string>> Sorted_Names_in { get; } = [];
    public List<string>? Missed_Protocols_in { get; set; }
    public List<string>? Unknown_Protocols_in { get; set; }

    public SimpleProtocolNames(Dictionary<string, List<FileInfo>> files)
    {
        numeric_numbers_in = GetProtocolNumbers(files);

        MinimumNumbers extreme_min_lcl = new(numeric_numbers_in);
        current_period_min_numbers_in = extreme_min_lcl.Numbers_in;

        SimpleSort sorter_lcl = new();

        foreach (var item in files)
        {
            Sorted_Names_in.Add(item.Key, sorter_lcl.Sorting(numeric_numbers_in[item.Key], item.Value));
        }

        ComputeMissedProtocols();
    }

    private Dictionary<string, List<int>> GetProtocolNumbers(Dictionary<string, List<FileInfo>> files)
    {
        Dictionary<string, List<int>> numbers_lcl = [];

        foreach (var item in files)
        {
            numbers_lcl.Add(item.Key, number_converter_in.ConvertToNumbers(item.Value));
        }

        return numbers_lcl;
    }

    private void ComputeMissedProtocols()
    {
        MaximumNumbers extreme_max_lcl = new(numeric_numbers_in);
        var max_numbers_lcl = extreme_max_lcl.Numbers_in;

        List<string> missed_protocols_lcl = [];

        foreach (var item in numeric_numbers_in)
        {
            if (item.Value.Count >= 2)
            {
                var range_lcl = Enumerable.Range(current_period_min_numbers_in[item.Key] + 1, max_numbers_lcl[item.Key] - current_period_min_numbers_in[item.Key]);

                IEnumerable<int> missed_lcl = range_lcl.Except(item.Value);
                List<int> missed_numbers_lcl = [.. missed_lcl];

                foreach (int number in missed_numbers_lcl)
                {
                    missed_protocols_lcl.Add($"{number}-{GetShortTypeName(item.Key)}");
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

    public void ComputeUnknownProtocols(Dictionary<string, List<FileInfo>> previous_period_files)
    {
        MaximumNumbers previous_extreme_max_lcl = new(GetProtocolNumbers(previous_period_files));
        var numbers_lcl = previous_extreme_max_lcl.Numbers_in;

        List<string> unknown_protocols_lcl = [];

        foreach (var item in current_period_min_numbers_in!)
        {
            int min_number_lcl = numbers_lcl[item.Key];
            int max_number_lcl = item.Value;

            bool unknowns_found_status = (min_number_lcl < max_number_lcl) && ((max_number_lcl - 1) != min_number_lcl);

            if (unknowns_found_status)
            {
                for (int start_num = min_number_lcl + 1; start_num < max_number_lcl; start_num++)
                {
                    unknown_protocols_lcl.Add($"{start_num}{Symbols.LINE}{GetShortTypeName(item.Key)}");    
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
