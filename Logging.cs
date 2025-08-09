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


    abstract class MonthLogData
    {
        private readonly XDocument xlog = XDocument.Load(AppConstants.logs_file);

        private protected XElement? GetMonthData(int month_value)
        {
            return xlog.Element("logs_data")?.Elements("month").FirstOrDefault(p => p.Attribute("value")?.Value == $"{month_value}");
        }
    }

    // только на каждый месяц 
    class MonthLogger : MonthLogData, IProtocolNumbers, ISortedNames
    {
        private readonly int month_value;
        private readonly List<FileInfo> eias_files;
        private readonly Dictionary<string, List<FileInfo>> simple_files;

        private readonly List<string>? sorted_eias_names;
        private readonly Dictionary<string, List<string>>? sorted_simple_names;

        public MonthLogger((int month_value, List<FileInfo>? eias_files, Dictionary<string, List<FileInfo>>? simple_files, MonthSums backup_log) full_backup)
        {
            // write !





            if (full_backup.backup_log.All_Protocols[AppConstants.others_sums[1]] > 0)
            {
                eias_files = full_backup.eias_files!;

                sorted_eias_names = ISortedNames.CreateSortedNames(IProtocolNumbers.ConvertToNumbers(eias_files, AppConstants.eias_number_pattern), eias_files);
            }

            if (full_backup.backup_log.All_Protocols[AppConstants.others_sums[2]] > 0)
            {
                simple_files = full_backup.simple_files!;

                sorted_simple_names = [];




            }


        }








        

        public int CopyBackupFiles(string target_directory) // month dir
        {
            int files_count = 0;

            for (int file_index = 0; file_index < files.Count; file_index++)
            {
                var new_file = files[file_index].CopyTo(target_directory, true);   // exception !!
                Console.WriteLine($"\n{new_file.FullName} успешно скопирован !");
                files_count++;
            }
            return files_count;
        }



        private void CreateSortedSimpleNames()
        {
            foreach (var item in full_backup.simple_files)
            {
                var current_numbers = full_backup.backup_log.Self_Obj_Currents_Type_Numbers!.Numbers[item.Key];
                var current_files = item.Value;

                sorted_simple_names.Add(item.Key, ISortedNames.CreateSortedNames(current_numbers, current_files));
            }
        }




    }
}

