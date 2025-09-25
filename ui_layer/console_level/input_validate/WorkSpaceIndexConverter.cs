// * Файл "WorkSpaceIndexConverter.cs": класс для получения индекса рабочего пространства из введенных символов. Только 1, 2 или 3. *

class WorkSpaceIndexConverter
{
    public static int Index_in
    {
        get
        {
            var input_symbol = Console.ReadKey(intercept: true).Key;

            if (input_symbol is ConsoleKey.D1 or ConsoleKey.NumPad1)
            {
                return (int)WorkSpaceIndex.SOURCE_INDEX;
            }
            else if (input_symbol is ConsoleKey.D2 or ConsoleKey.NumPad2)
            {
                return (int)WorkSpaceIndex.DESTINATION_INDEX;
            }
            else if (input_symbol is ConsoleKey.D3 or ConsoleKey.NumPad3)
            {
                return (int)WorkSpaceIndex.LOG_INDEX;
            }
            else
            {
                return (int)WorkSpaceIndex.NOT_SPACE_INDEX;
            }
        }
    }
}
