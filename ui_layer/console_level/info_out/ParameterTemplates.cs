// * Файл "ParameterTemplates.cs": класс шаблон вывода параметров выбора. *

static class ParameterTemplates
{
    // Вывод списка параметров <out_info>.

    public static void ShowParameters(List<string> out_info)
    {
        Console.WriteLine($" | {string.Join("\n | ", out_info)}");
    }

    // Создание параметра цифры по индексу значения.
    // Параметры: список из которого получить индекс, значение параметра.

    public static string CreateParameterDigit(List<string> parameters_type, string indexing_value)
    {
        return $"[{parameters_type.IndexOf(indexing_value) + 1}]";
    }
}
