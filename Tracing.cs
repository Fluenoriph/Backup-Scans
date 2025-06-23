using System.Xml.Linq;
//using System.Text.RegularExpressions;
using BackupBlock;
using DrivesControl;
using Logging;


namespace Tracing
{
    class BackupProcess(string period_month)
    {
        private const string simple_file_pattern = "^\\d{1,4}-(ф|фа|р|ра|м|ма)-";
        private const string eias_file_pattern = "^\\d{5}-\\d{2}-\\d{2}-";

        

        private List<RgxPattern> CreateRgxPatterns()
        {
            int month_value = MonthValues.Table[period_month];
            List<RgxPattern> patterns = [new(month_value - 1, simple_file_pattern), new(month_value, simple_file_pattern), new(month_value, eias_file_pattern)];
            // если январь, то возвратим список из 2 паттернов
            if (month_value == 1)
            {
                patterns.RemoveAt(0);
                return patterns;         // что за место 0 индекса ??
            }
            // если не январь, то возвращаем 3 паттерна в списке, начиная с предыдущего месяца
            else
            {
                return patterns;
            }
        }





    }




    // калькуляция всех сумм








}
