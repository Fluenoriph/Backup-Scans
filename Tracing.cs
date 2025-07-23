using BackupBlock;
using DrivesControl;
using Logging;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace Tracing
{
    readonly struct ProtocolFullTypeLocation
    {
        public static List<string> others_sums = ["Всего", "ЕИАС", "Простые"];
               
        public static List<string> location_sums = ["Уссурийск всего", "Арсеньев всего"];
                
        public static List<string> type_full_sums = ["Физические факторы всего", "Радиационный контроль всего", "Измерения мебели всего"];

        public static List<string> type_location_sums = ["Физические факторы (Уссурийск)", "Физические факторы (Арсеньев)",
                                                           "Радиационный контроль (Уссурийск)", "Радиационный контроль (Арсеньев)",
                                                           "Измерения мебели (Уссурийск)", "Измерения мебели (Арсеньев)"];

        public static List<string> not_found_sums = ["Пропущенные", "Неизвестные"];

        public ProtocolFullTypeLocation() { }
    }


    abstract class GeneralSums
    {
        public string Period { get; set; } = "";
        public Dictionary<string, int> All_Protocols { get; } = [];

        public GeneralSums()
        {
            foreach (string item in ProtocolFullTypeLocation.others_sums)
            {
                All_Protocols.Add(item, 0);
            }
        }
    }


    interface ISimpleProtocolsSums
    {
        static Dictionary<string, int> CreateTable()
        {
            Dictionary<string, int> sums = [];

            void AddItem(List<string> list)
            {
                foreach (string item in list)
                {
                    sums.Add(item, 0);
                }
            }

            AddItem(ProtocolFullTypeLocation.location_sums);
            AddItem(ProtocolFullTypeLocation.type_full_sums);
            AddItem(ProtocolFullTypeLocation.type_location_sums);
            AddItem(ProtocolFullTypeLocation.not_found_sums);

            return sums;
        }
    }

        
    class MonthSums : GeneralSums
    {
        public Dictionary<string, int>? Simple_Protocols { get; set; }
        public List<string>? Missed_Protocols { get; set; }
        public List<string>? Unknown_Protocols { get; set; }
    }


    class YearSums : GeneralSums, ISimpleProtocolsSums 
    {
        public Dictionary<string, int> Simple_Protocols { get; }

        public YearSums()
        {
            Simple_Protocols = ISimpleProtocolsSums.CreateTable();
        }
    }


    class FileTransfer(List<FileInfo> backup_files)
    {
        private readonly List<FileInfo> files = backup_files;
          
        public int CopyBackupFiles(string target_directory) // month dir
        {
            int files_count = 0;

            for (int file_index = 0; file_index < files.Count; file_index++)
            {
                var new_file = files[file_index].CopyTo(target_directory, true);
                Console.WriteLine($"\n{new_file.FullName} успешно скопирован !");
                files_count++;
            }
            return files_count;
        }
    }

    // только анализ простых
    class ProtocolsAnalysis : ISimpleProtocolsSums
    {
        private protected List<int> current_period_min_numbers = [];
        private readonly ProtocolTypeNumbers obj_protocol_type_numbers;   // struct massive ???
                                
        public ProtocolsAnalysis(List<FileInfo> captured_simple_files)
        {
            obj_protocol_type_numbers = new(captured_simple_files);

            Simple_Protocols_Sums = ISimpleProtocolsSums.CreateTable();

            CalculateTypeSums();
            Missed_Protocols = ComputeMissedProtocols();
        }

        public Dictionary<string, int> Simple_Protocols_Sums { get; }

        public List<string>? Missed_Protocols { get; }
        
        private void CalculateTypeSums()  
        {
            // рассчет сумм типов протоколов и запись в словарь
            // >> рассчет всех типов и по району
            for (int type_index = 0; type_index < ProtocolFullTypeLocation.type_location_sums.Count; type_index++)
            {
                string current_protocol_type = ProtocolFullTypeLocation.type_location_sums[type_index];
                List<int>? current_protocol_numbers = obj_protocol_type_numbers.Numbers[type_index];

                if (current_protocol_numbers is not null)
                {
                    Simple_Protocols_Sums[current_protocol_type] = current_protocol_numbers.Count;
                }
            }
            // рассчет каждого типа всего
            for (int type_index = 0, calc_index = 0; type_index < ProtocolFullTypeLocation.type_full_sums.Count; type_index++)
            {
                Simple_Protocols_Sums[ProtocolFullTypeLocation.type_full_sums[type_index]] = Simple_Protocols_Sums[ProtocolFullTypeLocation.type_location_sums[calc_index]] + Simple_Protocols_Sums[ProtocolFullTypeLocation.type_location_sums[calc_index + 1]];
                calc_index += 2;
            }
            // рассчет по району                            
            for (int city_index = 0, calc_index = 0; city_index < ProtocolFullTypeLocation.location_sums.Count; city_index++)
            {
                Simple_Protocols_Sums[ProtocolFullTypeLocation.location_sums[city_index]] = Simple_Protocols_Sums[ProtocolFullTypeLocation.type_location_sums[calc_index]] + Simple_Protocols_Sums[ProtocolFullTypeLocation.type_location_sums[calc_index + 2]] + Simple_Protocols_Sums[ProtocolFullTypeLocation.type_location_sums[calc_index + 4]];
                calc_index += 1;
            }
        }
                
        private List<string>? ComputeMissedProtocols()
        {
            MinimumNumbers extreme_min = new(obj_protocol_type_numbers.Numbers);
            current_period_min_numbers = extreme_min.Numbers;

            MaximumNumbers extreme_max = new(obj_protocol_type_numbers.Numbers);
            List<int> max_numbers = extreme_max.Numbers;

            List<string> missed_protocols = [];

            for (int type_index = 0; type_index < SimpleProtocolTypes.types_count; type_index++)
            {
                List<int>? current_protocols = obj_protocol_type_numbers.Numbers[type_index];

                if ((current_protocols is not null) && (current_protocols.Count >= 2))
                {
                    var range = Enumerable.Range(current_period_min_numbers[type_index] + 1, max_numbers[type_index] - current_period_min_numbers[type_index]);
                    
                    IEnumerable<int> missing = range.Except(current_protocols); 
                    List<int> missing_numbers = [.. missing];

                    foreach (int number in missing_numbers)
                    {
                        missed_protocols.Add($"{number}-{SimpleProtocolTypes.protocol_types[type_index]}");
                    }
                }
            }

            if (missed_protocols.Count is not 0)
            {
                Simple_Protocols_Sums[ProtocolFullTypeLocation.not_found_sums[0]] = missed_protocols.Count;  
                return missed_protocols;
            }
            else
            {
                return null;
            }
        }
    }


    class AnalysWithUnknownProtocols : ProtocolsAnalysis
    {
        public AnalysWithUnknownProtocols(List<FileInfo> captured_simple_files, List<FileInfo>? previous_period_files) : base(captured_simple_files) 
        {
            if (previous_period_files is not null)
            {
                ProtocolTypeNumbers previous_type_numbers = new(previous_period_files);
                
                MaximumNumbers previous_max = new(previous_type_numbers.Numbers);

                Unknown_Protocols = ComputeUnknownProtocols(previous_max.Numbers, current_period_min_numbers);
            }
            else
            {
                Unknown_Protocols = null;
            }
        }

        public List<string>? Unknown_Protocols { get; }

        private List<string>? ComputeUnknownProtocols(List<int> previous_period_max_numbers, List<int> current_period_min_numbers)
        {
            List<string> unknown_protocols = [];

            for (int type_index = 0; type_index < SimpleProtocolTypes.types_count; type_index++)
            {
                string current_type = SimpleProtocolTypes.protocol_types[type_index];

                int min_number = previous_period_max_numbers[type_index];
                int max_number = current_period_min_numbers[type_index];

                bool unknowns_ok = (min_number < max_number) && ((max_number - 1) != min_number);

                if (unknowns_ok)
                {
                    for (int start_num = min_number + 1; start_num < max_number; start_num++)     
                    {
                        unknown_protocols.Add($"{start_num}-{current_type}");
                    }
                }
            }

            if (unknown_protocols.Count is not 0)
            {
                Simple_Protocols_Sums[ProtocolFullTypeLocation.not_found_sums[1]] = unknown_protocols.Count;  
                return unknown_protocols;
            }
            else
            {
                return null;
            }
        }
    }

    
    abstract class BackupFiles(PdfFiles self_obj_source_files)
    {
        public List<FileInfo>? CapturingFiles(string file_pattern, int month_value)
        {
            Regex pattern = new(string.Concat(file_pattern, "\\d{2}\\.", $"0{month_value}", "\\.", CurrentYear.Year, "\\.", FileTypesPatterns.file_types["PDF"], "$"), RegexOptions.IgnoreCase);

            return self_obj_source_files.GrabMatchedFiles(pattern);
        }
    }


    class BackupFilesMonth(PdfFiles self_obj_source_files) : BackupFiles(self_obj_source_files)
    {
        private readonly List<List<FileInfo>?> files = [];
        public List<List<FileInfo>?>? Files { get; private set; }

        public void GetMonthBlock(int month_value) 
        {
            int null_count = 0;          

            foreach (string pattern_type in FileTypesPatterns.protocol_file_type)
            {
                var capturing_files = CapturingFiles(FileTypesPatterns.file_patterns[pattern_type], month_value);
                if (capturing_files is null)
                {
                    null_count++;
                }
                files.AddRange(capturing_files);
            }

            int NULL_FILES = 2;

            if (null_count != NULL_FILES)
            {
                Files = files;
            }
            else
            {
                Files =  null;
            }
        }
    }

    




    /*class BackupFilesYear : BackupFiles
    {
        public Dictionary<string, List<List<FileInfo>?>>? Files { get; }
        private readonly Dictionary<string, List<List<FileInfo>?>> files = [];
        
        public BackupFilesYear(PdfFiles self_obj_source_files) : base(self_obj_source_files)
        {
            foreach (string protocol_type in FileTypesPatterns.protocol_file_type)
            {
                files.Add(protocol_type, []);
            }
            
            int null_count = 0;

            for (int month_index = 1; month_index < MonthValues.Month_Count; month_index++)  
            {
                foreach (string type in FileTypesPatterns.protocol_file_type)
                {
                    var month_files = CapturingFiles(FileTypesPatterns.file_patterns[type], month_index);

                    if (month_files is null)
                    {
                        null_count++;
                    }
                    files[type].AddRange(month_files);
                }
            }

            NULL_FILES = 24;

            if (null_count != NULL_FILES)
            {
                Files = files;
            }
            else
            {
                Files = null;
            }
        }
    }*/
}
// проверено пока