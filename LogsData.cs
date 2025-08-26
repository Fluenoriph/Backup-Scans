using BackupBlock;
using System.Globalization;
using System.Xml.Linq;
using TextData;


namespace Logging
{
    interface IXMLSector
    {
        static XElement Create(string sector_name, List<string> tags)
        {
            XElement element = new(sector_name);

            foreach (string tag in tags)
            {
                element.Add(new XElement(tag));
            }

            return element;
        }
    }


    struct SumsSector : IXMLSector
    {
        public static List<string> United_Tags = [.. AppConstants.others_sums_tags, .. AppConstants.simple_sums_tags];
        public static XElement sums = IXMLSector.Create(AppConstants.sums_tag, United_Tags);
    }


    abstract class LogFile
    {
        public XDocument Document { get; }
        public string Filename { get; }
        abstract private protected XElement? Root { get; }
                
        public LogFile(string file)
        {
            Filename = file;
            FileInfo log_file = new(file);

            if (log_file.Exists)
            {
                Document = XDocument.Load(log_file.FullName);
            }
            else
            {
                Document = new();
                Document.Add(Root);
                Document.Save(log_file.FullName);   
            }
        }        
    }


    class XDrivesConfigFile(string file) : LogFile(file), IXMLSector
    {
        private protected override XElement? Root { get; } = IXMLSector.Create(AppConstants.drives_config_tag, AppConstants.drive_tags);
    }


    class XYearLogFile(string file) : LogFile(file), IXMLSector
    {
        private protected override XElement? Root { get; } = SumsSector.sums;
    }


    class XMonthLogFile(string file) : LogFile(file), IXMLSector
    {
        private protected override XElement? Root { get; } = CreateMonthLevels();

        public XElement? GetMonthData(string month)
        {
            return Document.Root!.Elements("month").FirstOrDefault(x_month => x_month.Attribute("name")?.Value == month);
        }       

        private static XElement CreateMonthLevels()
        {
            XElement root = new("logs_data");

            foreach (string month in AppConstants.month_names)
            {
                XElement x_month = new("month");
                XAttribute current_month = new("name", month);

                x_month.Add(current_month);
                x_month.Add(SumsSector.sums);
                x_month.Add(IXMLSector.Create(AppConstants.names_tag, [AppConstants.others_sums_tags[1], .. AppConstants.type_tags, AppConstants.simple_sums_tags[11], AppConstants.simple_sums_tags[12]]));
                
                root.Add(x_month);
            }

            return root;
        }
    }
          

    class Drive
    {
        public string? Directory_Name { get; set; }
        public DirectoryInfo? Directory { get; set; }
        
        public bool Directory_Exist 
        { 
            get
            {
                var dir_check = string.IsNullOrEmpty(Directory_Name);

                if (!dir_check)
                {
                    Directory = new(Directory_Name!);

                    if (Directory.Exists)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
    }
   

    class DrivesControl
    {
        public Drive Drive { get; set; }

        public DrivesControl(string drive_type)
        {
            // если повреждение тэга, то исключение, если просто другое имя то 'null' -- exit
            // проверить на исключение при повреждении имен дисков (тэга) -- exit
            // получаем конфигурацию

            XDrivesConfigFile self_obj_x_drives = new(AppConstants.drives_config_file);
            var root_level = self_obj_x_drives.Document!.Root;

            var target_x_drive = root_level?.Element(drive_type);            
            var directory = target_x_drive?.Value;

            bool directory_status;

            // проверка существования директории в системе

            Drive = new();

            do
            {
                Drive.Directory_Name = directory;
                directory_status = Drive.Directory_Exist;
                                
                if (!directory_status)
                {
                    AppInfoConsoleOut.ShowDirectoryExistFalse(drive_type);
                    AppInfoConsoleOut.ShowLine();

                    directory = InputNoNullText.GetRealText();

                    target_x_drive!.Value = directory;
                    self_obj_x_drives.Document.Save(AppConstants.drives_config_file);

                    Console.WriteLine('\n');
                    AppInfoConsoleOut.ShowInstallDirectory(drive_type);
                    Console.WriteLine('\n');
                }
                 
            } while (directory_status == false);   
        }
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
                        sum.Value = sums[sums_names[sum_index]].ToString(CultureInfo.CurrentCulture);
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
        public YearLogger(XYearLogFile log_file, Dictionary<string, int> all_sums, Dictionary<string, int> simple_sums)
        {
            Sums_Sector = log_file.Document.Root;   

            WriteSums(AppConstants.others_sums_tags, all_sums, AppConstants.others_sums);
            WriteSums(AppConstants.simple_sums_tags, simple_sums, AppConstants.united_simple_type_names);

            log_file.Document.Save(log_file.Filename);
        }       
    }

    
    class MonthLogger : XSumsData
    {        
        private XElement? Protocol_Names_Sector { get; set; }

        public MonthLogger(XMonthLogFile log_file, string month, MonthBackupSums self_obj_sums, List<FileInfo>? eias_files)
        {
            var month_sector = log_file.GetMonthData(month);

            Sums_Sector = month_sector?.Element(AppConstants.sums_tag);       
            Protocol_Names_Sector = month_sector?.Element(AppConstants.names_tag);

            WriteSums(AppConstants.others_sums_tags, self_obj_sums.All_Protocols, AppConstants.others_sums);

            if (self_obj_sums.All_Protocols[AppConstants.others_sums[1]] != 0)
            {
                EIASConvert self_obj_number_convert = new();
                EIASSort self_obj_name_sort = new();

                WriteNames(AppConstants.others_sums_tags[1], self_obj_name_sort.Sorting(self_obj_number_convert.ConvertToNumbers(eias_files!), eias_files!));
            }
            else
            {
                WriteNames(AppConstants.others_sums_tags[1]);
            }

            if (self_obj_sums.All_Protocols[AppConstants.others_sums[2]] != 0)
            {
                WriteSums(AppConstants.simple_sums_tags, self_obj_sums.Simple_Protocols_Sums, AppConstants.united_simple_type_names);
                                
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

            log_file.Document.Save(log_file.Filename);
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


    class YearLogResultCalculate
    {
        private readonly List<int> calculated_sums = [];
        
        public YearLogResultCalculate(XMonthLogFile x_month_file, XYearLogFile x_year_file)
        {
            foreach (string sum_tag in SumsSector.United_Tags)
            {
                int sum_count = 0;

                foreach (string month_name in AppConstants.month_names)
                {
                    var current_month_sum_value = x_month_file.GetMonthData(month_name)?.Element(AppConstants.sums_tag)?.Element(sum_tag)?.Value;
                                        
                    bool is_value = (current_month_sum_value is not null) && (current_month_sum_value is not "0") && (current_month_sum_value is not "");

                    if (is_value)
                    {
                        sum_count += Convert.ToInt32(current_month_sum_value!, CultureInfo.CurrentCulture);
                    }
                }
                                
                var current_year_sum = x_year_file.Document.Root?.Element(sum_tag);

                if (current_year_sum is not null)
                {
                    current_year_sum.Value = sum_count.ToString(CultureInfo.CurrentCulture);
                }
                else
                {
                    // xml error exit ??
                }

                calculated_sums.Add(sum_count);
            }

            x_year_file.Document.Save(x_year_file.Filename);
        }

        public (Dictionary<string, int>, Dictionary<string, int>?) GetYearSums()
        {
            var all_sums = IGeneralSums.CreateTable();
            
            for (int sum_index = 0; sum_index < all_sums.Count; sum_index++)
            {
                all_sums[AppConstants.others_sums[sum_index]] = calculated_sums.GetRange(0, 3)[sum_index];
            }

            Dictionary<string, int>? simple_sums = null;  

            if (calculated_sums[2] != 0)
            {
                simple_sums = ISimpleProtocolsSums.CreateTable();

                for (int sum_index = 0; sum_index < simple_sums.Count; sum_index++)
                {
                    simple_sums[AppConstants.united_simple_type_names[sum_index]] = calculated_sums.GetRange(3, 13)[sum_index];
                }
            }
                    
            return (all_sums, simple_sums);
        }
    }
}
