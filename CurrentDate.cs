struct CurrentDate
{
    public static int Year   // day....
    {
        get
        {
            DateTime current_date = DateTime.Now;
            return current_date.Year;
        }
    }
}
