interface ISums
{
    static Dictionary<string, int> CreateTable(List<string> sums_type)
    {
        Dictionary<string, int> sums_lcl = [];

        sums_type.ForEach(item => sums_lcl.Add(item, 0));

        return sums_lcl;
    }
}
