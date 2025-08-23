using BackupBlock;
using System.Xml.Linq;
using TextData;


namespace Logging
{
    struct XMonthData
    {
        public static IEnumerable<XElement>? x_monthes = XLogFiles.x_doc_monthes.Element("logs_data")?.Elements("month");
    }


    abstract class XSumsData
    {
        private protected XElement? Sums_Sector { get; set; }
        
        private protected void WriteSums(List<string> tags, Dictionary<string, int>? sums = null, List<string>? sums_names = null)
        {
            for (int sum_index = 0; sum_index < tags.Count; sum_index++)
            {
                var sum = Sums_Sector!.Element(tags[sum_index]);

                if (sum is not null)
                {
                    if ((sums is not null) && (sums_names is not null))
                    {
                        sum.Value = sums[sums_names[sum_index]].ToString();
                    }
                    else
                    {
                        sum.Value = '0'.ToString();
                    }
                }
                else
                {
                    // error
                }
            }
        }                
    }

        
    class YearLogger : XSumsData
    {
        public YearLogger(Dictionary<string, int> all_sums, Dictionary<string, int> simple_sums)
        {
            Sums_Sector = XLogFiles.x_doc_year.Element(AppConstants.sums_tag);

            WriteSums(AppConstants.others_sums_tags, all_sums, AppConstants.others_sums);
            WriteSums(AppConstants.simple_sums_tags, simple_sums, AppConstants.united_type_names);
                        
            XLogFiles.x_doc_year.Save(AppConstants.year_log_file);
        }       
    }

    
    class MonthLogger : XSumsData
    {        
        private XElement? Protocol_Names_Sector { get; set; }

        public MonthLogger(string month, MonthBackupSums self_obj_sums, List<FileInfo>? eias_files)
        {        
            var month_sector = XMonthData.x_monthes?.FirstOrDefault(x_month => x_month.Attribute("name")?.Value == month);

            Sums_Sector = month_sector?.Element(AppConstants.sums_tag);       
            Protocol_Names_Sector = month_sector?.Element(AppConstants.names_tag);

            if (self_obj_sums.All_Protocols[AppConstants.others_sums[1]] != 0)
            {
                WriteSums(AppConstants.others_sums_tags, self_obj_sums.All_Protocols, AppConstants.others_sums);

                EIASConvert self_obj_number_convert = new();
                EIASSort self_obj_name_sort = new();

                WriteNames(AppConstants.others_sums_tags[1], self_obj_name_sort.Sorting(self_obj_number_convert.ConvertToNumbers(eias_files!), eias_files!));
            }
            else
            {
                WriteSums(AppConstants.others_sums_tags);
                WriteNames(AppConstants.others_sums_tags[1]);
            }

            if (self_obj_sums.All_Protocols[AppConstants.others_sums[2]] != 0)
            {
                WriteSums(AppConstants.simple_sums_tags, self_obj_sums.Simple_Protocols_Sums, AppConstants.united_type_names);
                                
                foreach (string name in AppConstants.types_full_names)
                {
                    var target_x_tag = AppConstants.type_tags[AppConstants.types_full_names.IndexOf(name)];

                    if (self_obj_sums.self_obj_names!.Sorted_Names.TryGetValue(name, out List<string>? value))
                    {
                        WriteNames(target_x_tag, value);
                    }
                    else
                    {
                        WriteNames(target_x_tag);
                    }
                }

                WriteNames(AppConstants.simple_sums_tags[11], self_obj_sums.self_obj_names!.Missed_Protocols);
                WriteNames(AppConstants.simple_sums_tags[12], self_obj_sums.self_obj_names!.Unknown_Protocols);
            }
            else
            {
                WriteSums(AppConstants.simple_sums_tags);

                foreach (var tag in AppConstants.type_tags)
                {
                    WriteNames(tag);
                }

                WriteNames(AppConstants.simple_sums_tags[11]);
                WriteNames(AppConstants.simple_sums_tags[12]);
            }

            XLogFiles.x_doc_monthes.Save(AppConstants.month_logs_file);
        }

        private void WriteNames(string tag, List<string>? names = null)
        {
            var x_names = Protocol_Names_Sector?.Element(tag);

            if (x_names is not null)
            {
                if (names is not null)
                {
                    x_names.Value = string.Join(", ", names);
                }
                else
                {
                    x_names.Value = string.Empty;
                }
            }
            else
            {
                // xml error
            }
        }
    }
}
