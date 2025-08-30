class MonthBackupSums
{
    public readonly SimpleProtocolNames? names_in;
    public Dictionary<string, int> All_Protocols_Sums_in { get; } = ISumsTableCreator.Create(ProtocolTypesSums.OTHERS_SUMS);
    public Dictionary<string, int>? Simple_Protocols_Sums_in { get; }

    public MonthBackupSums(List<FileInfo>? eias_files, Dictionary<string, List<FileInfo>>? simple_files, Dictionary<string, List<FileInfo>>? previous_period_simple_files = null)
    {
        if (eias_files is not null)
        {
            All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[0]] += eias_files.Count;
            All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[1]] = eias_files.Count;
        }

        if (simple_files is not null)
        {
            Simple_Protocols_Sums_in = ISumsTableCreator.Create(ProtocolTypesSums.UNITED_SIMPLE_TYPE_SUMS);

            foreach (var item in simple_files)
            {
                Simple_Protocols_Sums_in![item.Key] = item.Value.Count;
                All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[2]] += item.Value.Count;
            }
            All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[0]] += All_Protocols_Sums_in[ProtocolTypesSums.OTHERS_SUMS[2]];
            CalcProtocolTypeFullSum();
            CalcProtocolLocationSums();

            names_in = new(simple_files);

            if (previous_period_simple_files is not null)
            {
                names_in.ComputeUnknownProtocols(previous_period_simple_files);
            }

            CalcNotFoundProtocolsSums();
        }
    }

    private void CalcProtocolTypeFullSum()
    {
        for (int type_index = 0, calc_index = 0; type_index < ProtocolTypesSums.FULL_TYPE_SUMS.Count; type_index++)
        {
            Simple_Protocols_Sums_in![ProtocolTypesSums.FULL_TYPE_SUMS[type_index]] = Simple_Protocols_Sums_in[ProtocolTypesSums.TYPES_FULL_NAMES[calc_index]] + Simple_Protocols_Sums_in[ProtocolTypesSums.TYPES_FULL_NAMES[calc_index + 1]];
            calc_index += 2;
        }
    }

    private void CalcProtocolLocationSums()
    {
        for (int city_index = 0, calc_index = 0; city_index < ProtocolTypesSums.FULL_LOCATION_SUMS.Count; city_index++)
        {
            Simple_Protocols_Sums_in![ProtocolTypesSums.FULL_LOCATION_SUMS[city_index]] = Simple_Protocols_Sums_in[ProtocolTypesSums.TYPES_FULL_NAMES[calc_index]] + Simple_Protocols_Sums_in[ProtocolTypesSums.TYPES_FULL_NAMES[calc_index + 2]] + Simple_Protocols_Sums_in[ProtocolTypesSums.TYPES_FULL_NAMES[calc_index + 4]];
            calc_index += 1;
        }
    }

    private void CalcNotFoundProtocolsSums()
    {
        if (names_in!.Missed_Protocols_in is not null)
        {
            Simple_Protocols_Sums_in![ProtocolTypesSums.NOT_FOUND_SUMS[0]] = names_in.Missed_Protocols_in.Count;
        }

        if (names_in.Unknown_Protocols_in is not null)
        {
            Simple_Protocols_Sums_in![ProtocolTypesSums.NOT_FOUND_SUMS[1]] = names_in.Unknown_Protocols_in.Count;
        }
    }
}
