// * 

interface IYearSumsDataTypeConverter
{
    public static (Dictionary<string, int>, Dictionary<string, int>?) GetYearSums(List<int> all_sums)
    {
        // Создание словаря общих сумм. 

        var all_protocol_sums_lcl = ISumsTableCreator.Create(ProtocolTypesAndSums.MAIN_SUMS);

        for (int sum_index = 0; sum_index < all_protocol_sums_lcl.Count; sum_index++)
        {
            all_protocol_sums_lcl[ProtocolTypesAndSums.MAIN_SUMS[sum_index]] = all_sums.GetRange(0, 3)[sum_index];
        }

        // Простых протоколов может и не быть. Если они есть, то создаем словарь.

        Dictionary<string, int>? simple_protocol_sums_lcl = null;

        if (all_sums[2] != 0)
        {
            simple_protocol_sums_lcl = ISumsTableCreator.Create(ProtocolTypesAndSums.UNITED_SIMPLE_TYPE_SUMS);

            for (int sum_index = 0; sum_index < simple_protocol_sums_lcl.Count; sum_index++)
            {
                simple_protocol_sums_lcl[ProtocolTypesAndSums.UNITED_SIMPLE_TYPE_SUMS[sum_index]] = all_sums.GetRange(3, 13)[sum_index];
            }
        }

        return (all_protocol_sums_lcl, simple_protocol_sums_lcl);
    }
}
