using BackupBlock;
using System.Xml.Linq;
using TextData;


namespace Logging
{
    interface IXMLLogFile
    {
        XDocument Xdoc { get; }  // exception
    }


    abstract class SumsSector : IXMLLogFile
    {
        //public XDocument Xdoc { get; } = 
        private const string sums_tag = "sums";

        private protected XElement? Sums { get; }
                        
        private protected void Write(MonthSums backup_sums)  // field ?
        {
            for (int sum_index = 0; sum_index < AppConstants.others_sums_tags.Count; sum_index++)
            {
                var sum = Sums!.Element(AppConstants.others_sums_tags[sum_index])?.Value;
                sum = backup_sums.All_Protocols[AppConstants.others_sums[sum_index]].ToString();



                //Sums!.Element(AppConstants.others_sums_tags[sum_index]).Value = backup_sums.All_Protocols[AppConstants.others_sums[sum_index]].ToString();
                //sums.e[AppConstants.others_sums_tag[sum_index]].Value = "jj";

                //WriteSums(AppConstants.others_sums_tags[sum_index], all_sums[AppConstants.others_sums[sum_index]]);
            }

            if (backup_sums.Simple_Protocols_Sums is not null)
            {
                for (int sum_index = 0; sum_index < AppConstants.simple_sums_tags.Count; sum_index++)
                {
                    var sum = Sums!.Element(AppConstants.simple_sums_tags[sum_index]).Value;
                    sum = backup_sums.All_Protocols[AppConstants.united_type_names[sum_index]].ToString();


                    //WriteSums(AppConstants.simple_sums_tags[sum_index], simple_sums[AppConstants.united_type_names[sum_index]]);
                }
            }

        }


    }


    class YearLog
    {
        private XDocument? Xdoc { get; }

        public YearLog(Dictionary<string, int> all_sums, Dictionary<string, int>? simple_sums)
        {
            Xdoc = XDocument.Load(AppConstants.year_log_file);

            //var sums = Xdoc.Element("sums");




            

            Xdoc.Save(AppConstants.year_log_file);
        }

               
    }


    abstract class MonthLog : SumsSector
    {
        private protected XElement? protocol_names;
        
        private protected MonthLog(string month, MonthSums backup_sums)
        {
            Xdoc = XDocument.Load(AppConstants.month_logs_file);
            // условный null ?. 
            var month_data = GetMonthSector(month);

            if (month_data is not null)
            {
                Sums = GetSectorSums(month_data);

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

        private protected XElement? GetMonthSector(string month)
        {
            return Xdoc!.Element("logs_data")?.Elements("month").     //FirstOrDefault(p => p.Attribute("name")?.Value == month);
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
        public EIASLog(string month, List<FileInfo> files, MonthSums backup_sums) : base(month, backup_sums)
        {
            WriteSums(AppConstants.others_sums_tags[1], backup_sums.All_Protocols[AppConstants.others_sums[1]]);
                        
            EIASConvert self_obj_number_convert = new();
            EIASSort self_obj_name_sort = new();

            WriteNames(AppConstants.others_sums_tags[1], self_obj_name_sort.Sorting(self_obj_number_convert.ConvertToNumbers(files), files));
                            
            Xdoc!.Save(AppConstants.month_logs_file);
        }
    }


    class SimpleLog : MonthLog
    {
        public SimpleLog(string month, Dictionary<string, List<FileInfo>> files, MonthSums backup_sums) : base(month, backup_sums)
        {
            WriteSums(AppConstants.others_sums_tags[2], backup_sums.All_Protocols[AppConstants.others_sums[2]]);

            for (int sum_index = 0; sum_index < AppConstants.simple_sums_tags.Count; sum_index++)
            {
                WriteSums(AppConstants.simple_sums_tags[sum_index], backup_sums.Simple_Protocols_Sums![AppConstants.united_type_names[sum_index]]);
            }

            var type_tags = AppConstants.simple_sums_tags.GetRange(5, 6);

            SimpleSort self_obj_sorter = new();
            foreach (var item in backup_sums.Self_Obj_Currents_Type_Numbers!.Type_Numbers)
            {
                WriteNames(type_tags[AppConstants.types_full_names.IndexOf(item.Key)], self_obj_sorter.Sorting(item.Value, files[item.Key]));                                
            }

            if (backup_sums.Simple_Protocols_Sums![AppConstants.not_found_sums[0]] != 0)
            {
                WriteNames(AppConstants.simple_sums_tags[11], backup_sums.Missed_Protocols!);                
            }
            
            if (backup_sums.Simple_Protocols_Sums![AppConstants.not_found_sums[1]] != 0)
            {
                WriteNames(AppConstants.simple_sums_tags[12], backup_sums.Unknown_Protocols!);                
            }
                                   
            Xdoc!.Save(AppConstants.month_logs_file);
        }
    }                 
}
