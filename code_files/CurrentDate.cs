// * Файл CurrentDate.cs: структура для получения текущего года и представления даты и времени. *

using System.Globalization;


readonly struct CurrentDate
{
    static readonly DateTime current_date_in = DateTime.Now;

    public static int Year   
    {
        get
        {
            return current_date_in.Year;
        }
    }

    public static string DateAndTime
    {
        get
        {
            return current_date_in.ToString(CultureInfo.CurrentCulture);
        }
    }
}
