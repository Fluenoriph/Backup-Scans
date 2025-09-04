/* Классы для получения максимального и минимального номера протокола из словаря номеров.
   Предназначены только для протоколов по физическим факторам. */

// Параметр: номера протоколов, численные значения. 

abstract class ExtremeNumbers(Dictionary<string, List<int>> protocol_numbers)
{
    private readonly Dictionary<string, int> numbers_in = [];
    abstract private protected int GetExtremeNumber(List<int> current_numbers);

    // Создание коллекции номеров, крайних во множестве.

    public Dictionary<string, int> Numbers_in
    {
        get
        {
            foreach (var item in protocol_numbers)
            {
                // Вычисление граничного номера из списка номеров по типу протокола.

                numbers_in.Add(item.Key, GetExtremeNumber(item.Value));
            }

            return numbers_in;
        }
    }
}


class MaximumNumbers(Dictionary<string, List<int>> protocol_numbers) : ExtremeNumbers(protocol_numbers)
{
    private protected override int GetExtremeNumber(List<int> current_numbers)
    {
        return current_numbers.Last();
    }
}


class MinimumNumbers(Dictionary<string, List<int>> protocol_numbers) : ExtremeNumbers(protocol_numbers)
{
    private protected override int GetExtremeNumber(List<int> current_numbers)
    {
        return current_numbers.First();
    }
}
