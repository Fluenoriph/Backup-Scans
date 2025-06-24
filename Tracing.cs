using System.Xml.Linq;
//using System.Text.RegularExpressions;
using BackupBlock;
using DrivesControl;
using Logging;


namespace Tracing
{
    class BlockAnalysis(string period_month, FileInfo[] files_of_type)
    {
        private const string simple_file_pattern = "^\\d{1,4}-(ф|фа|р|ра|м|ма)-";  // >>
        private const string eias_file_pattern = "^\\d{5}-\\d{2}-\\d{2}-";         // ** public interface ??    in struct filesattype ???
        private const string file_type = "*.pdf";                                  // >>
        /// ********************************************************
        public Dictionary<string, int> Files_Sums { get; set; } = new()
        {
            ["Всего"] = 0,
            ["ЕИАС"] = 0,
            ["Простые"] = 0,
            ["Уссурийск всего"] = 0,
            ["Арсеньев всего"] = 0
        };

        public Dictionary<string, int> Protocol_Types_Sums { get; set; }



        private List<RgxPattern> CreateRgxPatterns()
        {
            int month_value = MonthValues.Table[period_month];
            List<RgxPattern> patterns = [new(simple_file_pattern, month_value - 1, file_type), new(simple_file_pattern, month_value, file_type), new(eias_file_pattern, month_value, file_type)];
            // если январь, то возвратим список из 2 паттернов
            if (month_value == 1)
            {
                patterns.RemoveAt(0);
                return patterns;         
            }
            // если не январь, то возвращаем 3 паттерна в списке, начиная с предыдущего месяца
            else
            {
                return patterns;
            }
        }





        private void SelfCalculation()
        {
            List<RgxPattern> target_file_patterns = CreateRgxPatterns();






        }



    }




    







}
