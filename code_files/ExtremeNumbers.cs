/*
 * Файл ExtremeNumbers.cs: граничные номера простых протоколов (ФФ).
 * 
 * 1. "BaseExtremeNumbers": базовый класс;
 * 2. "MaximumNumbers": класс для получения максимального номера протокола из словаря номеров;
 * 3. "MinimumNumbers": класс для получения минимального номера протокола из словаря номеров.
*/

// Параметр: номера протоколов, численные значения. 

abstract class BaseExtremeNumbers(Dictionary<string, List<int>> protocol_numbers)
{
    readonly Dictionary<string, int> numbers_in = [];
    abstract protected int GetExtremeNumber(List<int> current_numbers);

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


class MaximumNumbers(Dictionary<string, List<int>> protocol_numbers) : BaseExtremeNumbers(protocol_numbers)
{
    protected override int GetExtremeNumber(List<int> current_numbers)
    {
        return current_numbers.Last();
    }
}


class MinimumNumbers(Dictionary<string, List<int>> protocol_numbers) : BaseExtremeNumbers(protocol_numbers)
{
    protected override int GetExtremeNumber(List<int> current_numbers)
    {
        return current_numbers.First();
    }
}
