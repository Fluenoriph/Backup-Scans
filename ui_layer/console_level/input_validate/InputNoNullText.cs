// * Файл "InputNoNullText.cs": класс для проверки ввода пустой строки. *

class InputNoNullText
{
    // * Принудительно получить правильное значение. *

    public static string GetRealText()
    {
        string? text_lcl;
        bool real_text_status;

        // Цикл повторяется, до тех пор, пока не введут реальное значение.

        do
        {
            text_lcl = Console.ReadLine();
            real_text_status = string.IsNullOrEmpty(text_lcl);

            if (real_text_status)
            {
                Console.WriteLine($" {Symbols.GRILLE} Вы ничего не ввели, вводите заново !\n");
            }
        } while (real_text_status);

        return text_lcl!;
    }
}
