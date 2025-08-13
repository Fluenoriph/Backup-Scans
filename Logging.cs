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
        private protected readonly XDocument xlog = XDocument.Load(AppConstants.logs_file); // interface ??
        private protected XElement? sums;
        private protected XElement? protocol_names;    
             
        public MonthLogData(int month_value, MonthSums backup_sums)
        {
            var month_data = xlog.Element("logs_data")?.Elements("month").FirstOrDefault(p => p.Attribute("value")?.Value == $"{month_value - 1}");

            if (month_data is not null)
            {
                sums = month_data.Element("sums");

                if (sums is not null)
                {
                    var full_sum = sums.Element(AppConstants.others_sums_tags[0]);
                    if (full_sum is not null) full_sum.Value = $"{backup_sums.All_Protocols[AppConstants.others_sums[0]]}";
                }
                else
                {
                    // xml error
                }

                protocol_names = month_data.Element("protocol_names");

                if (protocol_names is null)
                {
                    // xml error
                }
            }
            else
            {
                // xml error
            }

        }
    }


    class EIASLog : MonthLogData
    {
        private readonly List<string> sorted_names;

        public EIASLog(int month_value, List<FileInfo> files, MonthSums backup_sums) : base(month_value, backup_sums)
        {
            var sum = sums!.Element(AppConstants.others_sums_tags[1]);
            if (sum is not null) sum.Value = $"{backup_sums.All_Protocols[AppConstants.others_sums[1]]}";

            sorted_names = ISortedNames.CreateSortedNames(IProtocolNumbers.ConvertToNumbers(files, AppConstants.eias_number_pattern), files);
            
            sorted_names.ForEach(name => Console.WriteLine(name));

            Console.WriteLine($"Names--{sorted_names.Count}");

            //var names = protocol_names!.Element(AppConstants.others_sums_tags[1]);
            //if (names is not null) names.Value = sorted_names[0];

            xlog.Save(AppConstants.logs_file);
        }
    }


    class SimpleLog : MonthLogData
    {
        private readonly Dictionary<string, List<string>> sorted_names = [];

        public SimpleLog(int month_value, Dictionary<string, List<FileInfo>> files, MonthSums backup_sums) : base(month_value, backup_sums)
        {
            var sum = sums!.Element(AppConstants.others_sums_tags[2]);
            if (sum is not null) sum.Value = $"{backup_sums.All_Protocols[AppConstants.others_sums[2]]}";




            for (int sum_index = 0; sum_index < AppConstants.simple_sums_tags.Count; sum_index++)
            {
                var type_sum = sums.Element(AppConstants.simple_sums_tags[sum_index]);
                if (type_sum is not null) type_sum.Value = $"{backup_sums.Simple_Protocols_Sums![SimpleSumsNames.United_Names[sum_index]]}";
            }


            // method abs sorted names
                        
            foreach (var item in files)
            {
                var current_numbers = backup_sums.Self_Obj_Currents_Type_Numbers!.Numbers[item.Key];
                var current_files = item.Value;
                List<string> type_names = [];

                foreach (int number in current_numbers)
                {
                    foreach (FileInfo protocol in current_files)
                    {
                        if (protocol.Name.StartsWith($"{number}")) 
                        {
                            type_names.Add(protocol.Name);
                            break;
                        }
                    }
                }

                sorted_names.Add(item.Key, type_names);
            }





            /*foreach (var item in sorted_names)
            {
                item.Value.ForEach(name => Console.WriteLine($"{name}"));
            }*/

            xlog.Save(AppConstants.logs_file);
        }
    }




            

                    

                    

                
            

    
}

