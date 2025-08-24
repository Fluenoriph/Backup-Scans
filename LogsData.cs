using BackupBlock;
using System.Xml.Linq;
using TextData;
using static System.Net.Mime.MediaTypeNames;


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
        public static XElement sums = IXMLSector.Create(AppConstants.sums_tag, [.. AppConstants.others_sums_tags, .. AppConstants.simple_sums_tags]);
    }


    abstract class LogFile
    {
        public XDocument? Document { get; }
        abstract private protected XElement? Root { get; }
                
        public LogFile(string file)
        {
            FileInfo log_file = new(file);

            if (log_file.Exists)
            {
                Document = XDocument.Load(log_file.FullName);
            }
            else
            {
                Document = new();
                Document.Add(Root);
                Document.Save(log_file.FullName);   // System.UnauthorizedAccessException  Access denied !
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


    class XMonthLogFile : LogFile, IXMLSector
    {
        private protected override XElement? Root { get; } = CreateMonthLevels();
        public IEnumerable<XElement>? X_Monthes { get; } 

        public XMonthLogFile(string file) : base(file)
        {
            X_Monthes = Document!.Root?.Elements("month");
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
        public string? Name { get; }
        public DirectoryInfo? Directory { get; }
        public bool Directory_Exist { get; }
        
        public Drive(string type, string? directory)
        {
            Name = type;

            var dir_check = string.IsNullOrEmpty(directory);

            if (!dir_check)
            {
                Directory = new(directory!);

                if (Directory.Exists)
                {
                    Directory_Exist = true;
                }
                else
                {
                    Directory_Exist = false;
                }
            }
            else
            {
                Directory_Exist = false;
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
            do
            {
                Drive = new(drive_type, directory);
                directory_status = Drive.Directory_Exist;
                                                
                if (directory_status)
                {
                    AppInfoConsoleOut.ShowDirectorySetupTrue(drive_type, directory!);
                    break;
                }
                else
                {
                    AppInfoConsoleOut.ShowDirectoryExistFalse(drive_type);
                    directory = InputNoNullText.GetRealText();
                                        
                    target_x_drive!.Value = directory;
                    self_obj_x_drives.Document.Save(AppConstants.drives_config_file);
                    AppInfoConsoleOut.ShowInstallDirectory(drive_type);
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
        public YearLogger(DirectoryInfo log_file_dir, Dictionary<string, int> all_sums, Dictionary<string, int> simple_sums)
        {
            string log_file = string.Concat(log_file_dir.FullName, '\\', AppConstants.year_log_file);
            XYearLogFile self_obj_log_file = new(log_file);

            Sums_Sector = self_obj_log_file.Document!.Root;

            WriteSums(AppConstants.others_sums_tags, all_sums, AppConstants.others_sums);
            WriteSums(AppConstants.simple_sums_tags, simple_sums, AppConstants.united_type_names);

            self_obj_log_file.Document.Save(log_file);
        }       
    }

    
    class MonthLogger : XSumsData
    {        
        private XElement? Protocol_Names_Sector { get; set; }

        public MonthLogger(DirectoryInfo log_file_dir, string month, MonthBackupSums self_obj_sums, List<FileInfo>? eias_files)
        {
            string log_file = string.Concat(log_file_dir.FullName, '\\', AppConstants.month_logs_file);
            XMonthLogFile self_obj_log_file = new(log_file);

            var month_sector = self_obj_log_file.X_Monthes?.FirstOrDefault(x_month => x_month.Attribute("name")?.Value == month);

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

            self_obj_log_file.Document!.Save(log_file);
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
