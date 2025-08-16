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


    interface IXmlLogFile
    {
        static XDocument GetXdoc(string log_file) 
        {
            return XDocument.Load(log_file);  // System.IO.FileNotFoundException
        }

        static XElement? GetSectorSums(XContainer source_sector)
        {
            return source_sector.Element("sums");
        }
    }


    abstract class SumsLog
    {
        private protected XDocument? xlog;
        private protected XElement? Sums { get; set; }

        private protected void WriteSums(string tag, int sum_value)
        {
            var sum = Sums!.Element(tag);

            if (sum is not null)
            {
                sum.Value = sum_value.ToString();
            }
            else
            {
                // xml error exit ??
            }
        }
    }


    class YearLog : SumsLog 
    {
        public YearLog(Dictionary<string, int> all_sums, Dictionary<string, int>? simple_sums)
        {
            xlog = IXmlLogFile.GetXdoc(AppConstants.year_log_file);
            Sums = IXmlLogFile.GetSectorSums(xlog);

            if (Sums is not null)
            {
                for (int sum_index = 0; sum_index < AppConstants.others_sums_tags.Count; sum_index++)
                {
                    WriteSums(AppConstants.others_sums_tags[sum_index], all_sums[AppConstants.others_sums[sum_index]]);
                }

                if (simple_sums is not null)
                {
                    for (int sum_index = 0; sum_index < AppConstants.simple_sums_tags.Count; sum_index++)
                    {
                        WriteSums(AppConstants.simple_sums_tags[sum_index], simple_sums[SimpleSumsNames.United_Names[sum_index]]);
                    }
                }

                xlog.Save(AppConstants.year_log_file);
            }
            else
            {
                // xml error exit ??
            }
        }
    }


    abstract class MonthLog : SumsLog
    {
        private protected XElement? protocol_names;
        
        public MonthLog(int month_value, MonthSums backup_sums)
        {
            xlog = IXmlLogFile.GetXdoc(AppConstants.month_logs_file);   // условный null ?. 

            var month_data = xlog.Element("logs_data")?.Elements("month").FirstOrDefault(p => p.Attribute("value")?.Value == $"{month_value - 1}");

            if (month_data is not null)
            {
                Sums = IXmlLogFile.GetSectorSums(month_data);

                if (Sums is not null)
                {
                    WriteSums(AppConstants.others_sums_tags[0], backup_sums.All_Protocols[AppConstants.others_sums[0]]);
                }
                else
                {
                    // xml error exit
                }

                protocol_names = month_data.Element("protocol_names");

                if (protocol_names is null)
                {
                    // xml error exit
                }
            }
            else
            {
                // xml error exit
            }
        }
                
        private protected void WriteNames(string tag, List<string> protocols)
        {
            var names = protocol_names!.Element(tag);

            if (names is not null)
            {
                names.Value = string.Join(", ", protocols);
            }
            else
            {
                // xml error exit ??
            }
        }
    }


    class EIASLog : MonthLog
    {
        public EIASLog(int month_value, List<FileInfo> files, MonthSums backup_sums) : base(month_value, backup_sums)
        {
            WriteSums(AppConstants.others_sums_tags[1], backup_sums.All_Protocols[AppConstants.others_sums[1]]);
                        
            EIASConvert self_obj_number_convert = new();
            EIASSort self_obj_name_sort = new();

            WriteNames(AppConstants.others_sums_tags[1], self_obj_name_sort.Sorting(self_obj_number_convert.ConvertToNumbers(files), files));
                            
            xlog!.Save(AppConstants.month_logs_file);
        }
    }


    class SimpleLog : MonthLog
    {
        public SimpleLog(int month_value, MonthSums backup_sums) : base(month_value, backup_sums)
        {
            WriteSums(AppConstants.others_sums_tags[2], backup_sums.All_Protocols[AppConstants.others_sums[2]]);

            for (int sum_index = 0; sum_index < AppConstants.simple_sums_tags.Count; sum_index++)
            {
                WriteSums(AppConstants.simple_sums_tags[sum_index], backup_sums.Simple_Protocols_Sums![SimpleSumsNames.United_Names[sum_index]]);
            }

            var type_tags = AppConstants.simple_sums_tags.GetRange(5, 6);

            foreach (var item in backup_sums.Self_Obj_Currents_Type_Numbers!.Sorted_Names)
            {
                WriteNames(type_tags[AppConstants.types_full_names.IndexOf(item.Key)], item.Value);                                
            }

            if (backup_sums.Simple_Protocols_Sums![AppConstants.not_found_sums[0]] != 0)
            {
                WriteNames(AppConstants.simple_sums_tags[11], backup_sums.Missed_Protocols!);                
            }
            
            if (backup_sums.Simple_Protocols_Sums![AppConstants.not_found_sums[1]] != 0)
            {
                WriteNames(AppConstants.simple_sums_tags[12], backup_sums.Unknown_Protocols!);                
            }
                                   
            xlog!.Save(AppConstants.month_logs_file);
        }
    }




            

                    

                    

                   
}

