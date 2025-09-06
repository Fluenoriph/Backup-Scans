/*
 * Файл NumberConverter.cs: выделение номера из имени протокола. 
 * 
 * 1. "BaseNumberConverter": базовый класс;
 * 2. "EIASConvert": класс для получения номера ЕИАС;
 * 3. "SimpleConvert": класс для получения номера простого протокола.
 */

using System.Globalization;
using System.Text.RegularExpressions;


abstract class BaseNumberConverter
{
    abstract protected string Protocol_Number_Pattern_in { get; }
    abstract protected string GetMatchedNumber(Match match);

    // * "Конвертация" файлов ("files"), в сортированный по возрастанию список номеров. *

    public List<int> ConvertToNumbers(List<FileInfo> files)
    {
        List<int> numbers_lcl = [];

        foreach (FileInfo protocol in files)
        {
            // Захват номера из имени по паттерну типа протокола.

            Match match_lcl = Regex.Match(protocol.Name, Protocol_Number_Pattern_in);

            if (match_lcl.Success)
            {
                numbers_lcl.Add(Convert.ToInt32(GetMatchedNumber(match_lcl), CultureInfo.CurrentCulture));
            }
        }

        // Сортировка по возрастанию номера результирующего списка.

        numbers_lcl.Sort();
        return numbers_lcl;
    }
}


// * Для протоколов ЕИАС (EIASConvert) и "физ. факторов" (SimpleConvert), разная логика получения номера. *

// "GetMatchedNumber": получение значения из объекта совпадения по группе захвата № 1 ("number").

class EIASConvert : BaseNumberConverter
{
    protected override string Protocol_Number_Pattern_in { get; } = FilePatterns.EIAS_NUMBER_PATTERN;

    protected override string GetMatchedNumber(Match match)
    {
        // Удаляем из имени символы тире: "-", для полного преобразования в число. Было 12345-01-25, стало 123450125.

#pragma warning disable CA1307
        return match.Groups[1].Value.Replace(Symbols.LINE.ToString(), string.Empty);
#pragma warning restore CA1307
    }
}


class SimpleConvert : BaseNumberConverter
{
    protected override string Protocol_Number_Pattern_in { get; } = FilePatterns.SIMPLE_NUMBER_PATTERN;
        
    protected override string GetMatchedNumber(Match match)
    {
        // Прямое получение номера.

        return match.Groups[1].Value;
    }
}
