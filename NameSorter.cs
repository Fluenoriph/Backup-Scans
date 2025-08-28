using System.Globalization;


abstract class NameSorter
{
    abstract private protected string GetNumberName(int number);

    public List<string> Sorting(List<int> numbers, List<FileInfo> files)
    {
        List<string> names_lcl = [];

        foreach (int number in numbers)
        {
            foreach (FileInfo protocol in files)
            {
#pragma warning disable CA1310
                if (protocol.Name.StartsWith(GetNumberName(number)))
#pragma warning restore CA1310
                {
                    names_lcl.Add(protocol.Name);
                    break;
                }
            }
        }
        return names_lcl;
    }
}


class SimpleSort : NameSorter
{
    private protected override string GetNumberName(int number)
    {
        return number.ToString(CultureInfo.CurrentCulture);
    }
}


class EIASSort : NameSorter
{
    private protected override string GetNumberName(int number)
    {
        string number_name_lcl = number.ToString(CultureInfo.CurrentCulture);

        number_name_lcl = number_name_lcl.Insert(5, Symbols.LINE.ToString());
        number_name_lcl = number_name_lcl.Insert(8, Symbols.LINE.ToString());

        return number_name_lcl;
    }
}
