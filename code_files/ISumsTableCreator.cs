// * Файл ISumsTableCreator.cs: интерфейс для создания словаря сумм. *

interface ISumsTableCreator
{
    static Dictionary<string, int> Create(List<string> key_names)
    {
        Dictionary<string, int> sums_lcl = [];

        // Все суммы устанавливаются в 0.

        key_names.ForEach(item => sums_lcl.Add(item, 0));

        return sums_lcl;
    }
}
