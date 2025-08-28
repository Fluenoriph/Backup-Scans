using System.Globalization;
using System.Text.RegularExpressions;


abstract class NumberConverter
{
    abstract private protected string Number_Pattern_in { get; }
    abstract private protected string NumberName(Match match);

    public List<int> ConvertToNumbers(List<FileInfo> files)
    {
        List<int> numbers_lcl = [];

        foreach (FileInfo protocol in files)
        {
            Match match_lcl = Regex.Match(protocol.Name, Number_Pattern_in);

            if (match_lcl.Success)
            {
                numbers_lcl.Add(Convert.ToInt32(NumberName(match_lcl), CultureInfo.CurrentCulture));
            }
        }

        numbers_lcl.Sort();
        return numbers_lcl;
    }
}


class SimpleConvert : NumberConverter
{
    private protected override string Number_Pattern_in { get; } = FilePatterns.SIMPLE_NUMBER_PATTERN;

    private protected override string NumberName(Match match)
    {
        return match.Groups[1].Value;
    }
}


class EIASConvert : NumberConverter
{
    private protected override string Number_Pattern_in { get; } = FilePatterns.EIAS_NUMBER_PATTERN;

    private protected override string NumberName(Match match)
    {
#pragma warning disable CA1307
        return match.Groups[1].Value.Replace(Symbols.LINE.ToString(), string.Empty);
#pragma warning restore CA1307
    }
}
