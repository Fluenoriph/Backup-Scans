abstract class ExtremeNumbers(Dictionary<string, List<int>> protocol_type_numbers)
{
    private readonly Dictionary<string, int> numbers_in = [];
    private protected abstract int GetExtremeNumber(List<int> current_numbers);

    public Dictionary<string, int> Numbers_in
    {
        get
        {
            foreach (var item in protocol_type_numbers)
            {
                numbers_in.Add(item.Key, GetExtremeNumber(item.Value));
            }

            return numbers_in;
        }
    }
}


class MaximumNumbers(Dictionary<string, List<int>> protocol_type_numbers) : ExtremeNumbers(protocol_type_numbers)
{
    private protected override int GetExtremeNumber(List<int> current_numbers)
    {
        return current_numbers.Last();
    }
}


class MinimumNumbers(Dictionary<string, List<int>> protocol_type_numbers) : ExtremeNumbers(protocol_type_numbers)
{
    private protected override int GetExtremeNumber(List<int> current_numbers)
    {
        return current_numbers.First();
    }
}
