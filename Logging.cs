using BackupBlock;
using System.Xml.Linq;
using TextData;
using Tracing;


namespace Logging
{
    class DrivesConfiguration
    {
        static DrivesConfiguration()
        {
            XDocument doc = XDocument.Load(AppConstants.drives_config_file);
            Config_Element = doc.Element("configuration");                 // если повреждение тэга, то исключение, если просто другое имя то 'null' -- exit
        }

        public static XElement? Config_Element { get; set; }
        
        public static void SetupDriveDirectory(string drive_name, string path)
        {
            XElement? dir = Config_Element?.Element(drive_name);

            if (dir is not null)
            {
                dir.Value = path;
                Config_Element?.Save(AppConstants.drives_config_file);

                Console.WriteLine($"\n{drive_name} is installed !!");     // out OK !!
            } // else ??
        }
    }


    abstract class MonthLogData : IProtocolNumbers, ISortedNames
    {
        private protected readonly XDocument xlog = XDocument.Load(AppConstants.logs_file); // interface ??
        
        private protected XElement? month_data;
        private protected readonly List<FileInfo>? eias_files;
        private protected readonly Dictionary<string, List<FileInfo>>? simple_files;
        private protected MonthSums backup_sums;

        private protected readonly List<string>? sorted_eias_names;
        private protected readonly Dictionary<string, List<string>>? sorted_simple_names;

        public MonthLogData((int month_value, List<FileInfo>? eias_files, Dictionary<string, List<FileInfo>>? simple_files, MonthSums backup_sums) full_backup)
        {
            month_data = xlog.Element("logs_data")?.Elements("month").FirstOrDefault(p => p.Attribute("value")?.Value == $"{full_backup.month_value}");
            
            backup_sums = full_backup.backup_sums;

            if (full_backup.eias_files is not null)
            {
                eias_files = full_backup.eias_files;

                sorted_eias_names = ISortedNames.CreateSortedNames(IProtocolNumbers.ConvertToNumbers(eias_files, AppConstants.eias_number_pattern), eias_files);
            }
        
            if (full_backup.simple_files is not null)
            {
                simple_files = full_backup.simple_files;
                sorted_simple_names = [];

                foreach (var item in simple_files!)
                {
                    var current_numbers = backup_sums.Self_Obj_Currents_Type_Numbers!.Numbers[item.Key];
                    var current_files = item.Value;

                    sorted_simple_names.Add(item.Key, ISortedNames.CreateSortedNames(current_numbers, current_files));
                }
            }
        }
    }

    // только на каждый месяц 
    class MonthLogger : MonthLogData
    {
        public MonthLogger((int month_value, List<FileInfo>? eias_files, Dictionary<string, List<FileInfo>>? simple_files, MonthSums backup_sums) full_backup) : base(full_backup)
        {
            // write !
            
            if (month_data is not null)
            {
                var sums = month_data.Element("sums");

                if (sums is not null)
                {
                    var full_sum = sums.Element(AppConstants.others_sums_tags[0]);
                    if (full_sum is not null) full_sum.Value = $"{backup_sums.All_Protocols[AppConstants.others_sums[0]]}";


                }


            }


            


        }

             


        




    }
}

