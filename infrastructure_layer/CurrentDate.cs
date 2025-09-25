// * Файл CurrentDate.cs: структура для получения текущего года и представления даты и времени. *

using System.Globalization;


readonly struct CurrentDate
{
    static readonly DateTime current_date_in = DateTime.Now;

    public static int Year_in { get => current_date_in.Year; }
    public static string Date_And_Time_in { get => current_date_in.ToString(CultureInfo.CurrentCulture); }

    // Печатное наименование года.

    public static string Current_Year_Print_in { get => string.Concat(CurrentDate.Year_in, " ", PeriodsNames.YEAR.ToLower(CultureInfo.CurrentCulture)); }
}
