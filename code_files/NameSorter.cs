/*
 * Файл NameSorter.cs: сортировка протоколов по возрастанию номера.
 * 
 * 1. "BaseNameSorter": базовый класс;
 * 2. "EIASSort": класс сортировщик протоколов ЕИАС;
 * 3. "SimpleSort": класс сортировщик простых протоколов.
 */

using System.Globalization;


abstract class BaseNameSorter
{
    abstract protected string GetNumberName(int number);

    // * Создание сортированного списка имен протоколов. *
    // Параметры: "numbers" - список сортированных номеров файлов протоколов "files".

    public List<string> Sorting(List<int> numbers, List<FileInfo> files)
    {
        List<string> sorted_names_lcl = [];

        // Перебор файлов протоколов на соответствие порядковому номеру.

        foreach (int number in numbers)
        {
            foreach (FileInfo protocol in files)
            {
                // Сортировка методом поиска, того файла, номер которого соответствует упорядоченному номеру.
                                
#pragma warning disable CA1310
                if (protocol.Name.StartsWith(GetNumberName(number)))
#pragma warning restore CA1310
                {
                    // При соответствии, имя добавляется в том порядке, в котором находятся сортированные номера.

                    sorted_names_lcl.Add(protocol.Name);
                    break;
                }
            }
        }
        return sorted_names_lcl;
    }
}


// * Для типов протоколов ЕИАС (EIASSort) и "физические факторы" (SimpleSort), разная реализация имени. * 

// "GetNumberName": получение действительного имени протокола из его численного представления номера ("number").

class EIASSort : BaseNameSorter
{
    // Вход: 123450125

    protected override string GetNumberName(int number)
    {
        string number_name_lcl = number.ToString(CultureInfo.CurrentCulture);

        // Так как, действительный номер может начинаться с нулей ("00123-01-25"), и при конвертации в число они исчезнут, то проверим количество цифр,
        // и при необходимости добавим в начало результирующей строки нули.
                
        int add_null_count = Symbols.EIAS_NUMBER_COUNT - number_name_lcl.Length;

        // Если нули отброшены при конвертации, то разница с действительным количеством цифр ЕИАС номера будет больше нуля.

        if (add_null_count != 0)
        {
            for (int add_null_index = 0; add_null_index < add_null_count; add_null_index++)
            {
                number_name_lcl = number_name_lcl.Insert(0, Symbols.NULL);
            }
        }

        number_name_lcl = number_name_lcl.Insert(5, Symbols.LINE.ToString());
        number_name_lcl = number_name_lcl.Insert(8, Symbols.LINE.ToString());

        // Выход: "12345-01-25"

        return number_name_lcl;
    }
}


class SimpleSort : BaseNameSorter
{
    // Вход: 123

    protected override string GetNumberName(int number)
    {
        // Выход: "123"

        return number.ToString(CultureInfo.CurrentCulture);
    }
}
