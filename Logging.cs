using BackupBlock;
using System.IO;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
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
        private protected readonly string name_separator = ", ";

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
        private readonly List<string> sorted_names = [];   // <T> field ??
        private readonly string line = "-";
        
        public EIASLog(int month_value, List<FileInfo> files, MonthSums backup_sums) : base(month_value, backup_sums)
        {
            var sum = sums!.Element(AppConstants.others_sums_tags[1]);
            if (sum is not null) sum.Value = $"{backup_sums.All_Protocols[AppConstants.others_sums[1]]}";
            //////////////////
            List<int> numbers = [];
            
            foreach (FileInfo protocol in files)
            {
                Match match = Regex.Match(protocol.Name, AppConstants.eias_number_pattern);

                if (match.Success)
                {
                    numbers.Add(Convert.ToInt32(match.Groups[1].Value.Replace(line, "")));
                }
            }
            numbers.Sort();

            foreach (int number in numbers)
            {
                foreach (FileInfo protocol in files)
                {
                    if (protocol.Name.StartsWith(CreateNumberName(number)))
                    {
                        sorted_names.Add(protocol.Name);
                        break;
                    }
                }
            }
                                   

            var names = protocol_names!.Element(AppConstants.others_sums_tags[1]);
            if (names is not null) names.Value = string.Join(name_separator, sorted_names);

            xlog.Save(AppConstants.logs_file);
        }

        private string CreateNumberName(int number) // lambda ?
        {
            string number_name = number.ToString();

            number_name = number_name.Insert(5, line);
            number_name = number_name.Insert(8, line);

            return number_name;
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

            //////////////////////////////
                       
            foreach (var item in sorted_names)
            {
                var names = protocol_names!.Element(AppConstants.simple_sums_tags.GetRange(5, 6)[AppConstants.types_full_names.IndexOf(item.Key)]);
                if (names is not null) names.Value = string.Join(name_separator, item.Value);   // если null это неверный тэг //test !!
            }

            if (backup_sums.Simple_Protocols_Sums![AppConstants.not_found_sums[0]] != 0)
            {
                var names = protocol_names!.Element(AppConstants.simple_sums_tags[11]);
                if (names is not null) names.Value = string.Join(name_separator, backup_sums.Missed_Protocols!);
            }
            // polymethod ???
            if (backup_sums.Simple_Protocols_Sums![AppConstants.not_found_sums[1]] != 0)
            {
                var names = protocol_names!.Element(AppConstants.simple_sums_tags[12]);
                if (names is not null) names.Value = string.Join(name_separator, backup_sums.Unknown_Protocols!);
            }




            /*foreach (var item in sorted_names)
            {
                item.Value.ForEach(name => Console.WriteLine($"{name}"));
            }*/

            xlog.Save(AppConstants.logs_file);
        }


    }




            

                    

                    

                
            

    
}

