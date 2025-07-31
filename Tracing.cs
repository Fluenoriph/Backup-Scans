using Aspose.Words.Drawing;
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


    interface IGeneralSums   
    {
        static Dictionary<string, int> CreateTable()
        {
            Dictionary<string, int> sums = [];

            foreach (string item in ProtocolFullTypeLocation.others_sums)
            {
                sums.Add(item, 0);
            }
            return sums;
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

    
    class MonthFilesBlock(List<FileInfo>? eias_files, List<FileInfo>? simple_files)
    {
        public List<FileInfo>? EIAS { get; } = eias_files;
        public List<FileInfo>? Simple { get; } = simple_files;
    }


    class BackupFilesMonth(PdfFiles self_obj_source_files)
    {
        private readonly Func<int, string> ConvertMonth = static (value) =>
        {
            if (value < 10)
            {
                return $"0{value}";
            }
            else
            {
                return value.ToString();
            }
        };

        public List<FileInfo>? CapturingFiles(string file_pattern, int month_value)
        {
            Regex pattern = new(string.Concat(file_pattern, "\\d{2}\\.", ConvertMonth(month_value), "\\.", CurrentYear.Year, "\\.", FileTypesPatterns.file_types["PDF"], "$"), RegexOptions.IgnoreCase);

            return self_obj_source_files.GrabMatchedFiles(pattern);
        }

        public MonthFilesBlock? GetFilesBlock(int month_value)
        {
            var eias = CapturingFiles(FileTypesPatterns.file_patterns[FileTypesPatterns.protocol_file_type[0]], month_value);
            var simple = CapturingFiles(FileTypesPatterns.file_patterns[FileTypesPatterns.protocol_file_type[1]], month_value);

            if ((eias is null) && (simple is null))
            {
                return null;
            }
            else
            {
                return new(eias, simple);
            }
        }
    }
        

    abstract class MonthSums : IGeneralSums
    {
        public Dictionary<string, int> All_Protocols { get; } = IGeneralSums.CreateTable();
                
        public MonthSums(MonthFilesBlock backup_block)
        {
            if (backup_block.EIAS is not null)
            {
                AddFullSum(backup_block.EIAS);
                All_Protocols[ProtocolFullTypeLocation.others_sums[1]] = backup_block.EIAS.Count;
            }

            if (backup_block.Simple is not null)
            {
                AddFullSum(backup_block.Simple);
                All_Protocols[ProtocolFullTypeLocation.others_sums[2]] = backup_block.Simple.Count;
            }
        }

        private protected void AddFullSum(List<FileInfo> files)
        {
            All_Protocols[ProtocolFullTypeLocation.others_sums[0]] += files.Count;
        }
    }


    class MonthSumsExceptUnknowns : MonthSums, ISimpleProtocolsSums
    {
        private protected Dictionary<string, int>? current_period_min_numbers;
        private protected readonly ProtocolTypeNumbers? self_obj_currents_type_numbers;
        private protected readonly Func<string, string> GetShortTypeName = (key) => SimpleProtocolTypes.protocol_types[ProtocolFullTypeLocation.type_location_sums.IndexOf(key)];

        public Dictionary<string, int>? Simple_Protocols_Sums { get; }
        public List<string>? Missed_Protocols { get; }

        public MonthSumsExceptUnknowns(MonthFilesBlock backup_block) : base(backup_block)
        {
            if (All_Protocols[ProtocolFullTypeLocation.others_sums[2]] != 0)
            {
                self_obj_currents_type_numbers = new(backup_block.Simple!);
                Simple_Protocols_Sums = ISimpleProtocolsSums.CreateTable();
                CalculateTypeSums();
                Missed_Protocols = ComputeMissedProtocols();
            }
        }

        private void CalculateTypeSums()
        {
            // рассчет сумм типов протоколов и запись в словарь
            // >> рассчет всех типов и по району
            foreach (var item in self_obj_currents_type_numbers!.Numbers)
            {
                Simple_Protocols_Sums![item.Key] = item.Value.Count;
            }
            // рассчет каждого типа всего
            for (int type_index = 0, calc_index = 0; type_index < ProtocolFullTypeLocation.type_full_sums.Count; type_index++)
            {
                Simple_Protocols_Sums![ProtocolFullTypeLocation.type_full_sums[type_index]] = Simple_Protocols_Sums[ProtocolFullTypeLocation.type_location_sums[calc_index]] + Simple_Protocols_Sums[ProtocolFullTypeLocation.type_location_sums[calc_index + 1]];
                calc_index += 2;
            }
            // рассчет по району                            
            for (int city_index = 0, calc_index = 0; city_index < ProtocolFullTypeLocation.location_sums.Count; city_index++)
            {
                Simple_Protocols_Sums![ProtocolFullTypeLocation.location_sums[city_index]] = Simple_Protocols_Sums[ProtocolFullTypeLocation.type_location_sums[calc_index]] + Simple_Protocols_Sums[ProtocolFullTypeLocation.type_location_sums[calc_index + 2]] + Simple_Protocols_Sums[ProtocolFullTypeLocation.type_location_sums[calc_index + 4]];
                calc_index += 1;
            }
        }

        private List<string>? ComputeMissedProtocols()
        {
            MinimumNumbers extreme_min = new(self_obj_currents_type_numbers!.Numbers);
            current_period_min_numbers = extreme_min.Numbers;

            MaximumNumbers extreme_max = new(self_obj_currents_type_numbers.Numbers);
            Dictionary<string, int> max_numbers = extreme_max.Numbers;

            List<string> missed_protocols = [];

            foreach (var item in self_obj_currents_type_numbers.Numbers)
            {
                if (item.Value.Count >= 2)
                {
                    var range = Enumerable.Range(current_period_min_numbers[item.Key] + 1, max_numbers[item.Key] - current_period_min_numbers[item.Key]);

                    IEnumerable<int> missing = range.Except(item.Value);
                    List<int> missing_numbers = [.. missing];

                    foreach (int number in missing_numbers)
                    {
                        missed_protocols.Add($"{number}-{GetShortTypeName(item.Key)}");
                    }
                }
            }

            if (missed_protocols.Count > 0)
            {
                Simple_Protocols_Sums![ProtocolFullTypeLocation.not_found_sums[0]] = missed_protocols.Count;
                return missed_protocols;
            }
            else
            {
                return null;
            }
        }
    }


    class MonthSumsWithUnknowns : MonthSumsExceptUnknowns
    {
        public List<string>? Unknown_Protocols { get; }

        public MonthSumsWithUnknowns(MonthFilesBlock backup_block, List<FileInfo>? previous_period_simple_files) : base(backup_block)
        {
            if ((self_obj_currents_type_numbers is not null) && (previous_period_simple_files is not null))  // test !!
            {
                ProtocolTypeNumbers self_obj_previous_type_numbers = new(previous_period_simple_files);

                MaximumNumbers self_obj_previous_max = new(self_obj_previous_type_numbers.Numbers);

                Unknown_Protocols = ComputeUnknownProtocols(self_obj_previous_max.Numbers, current_period_min_numbers!);
            }
            else
            {
                Unknown_Protocols = null;
            }
        }

        private List<string>? ComputeUnknownProtocols(Dictionary<string, int> previous_period_max_numbers, Dictionary<string, int> current_period_min_numbers)
        {
            List<string> unknown_protocols = [];

            foreach (var item in current_period_min_numbers)
            {
                int min_number = previous_period_max_numbers[item.Key];
                int max_number = item.Value;

                bool unknowns_ok = (min_number < max_number) && ((max_number - 1) != min_number);

                if (unknowns_ok)
                {
                    for (int start_num = min_number + 1; start_num < max_number; start_num++)
                    {
                        unknown_protocols.Add($"{start_num}-{GetShortTypeName(item.Key)}");
                    }
                }
            }

            if (unknown_protocols.Count is not 0)
            {
                Simple_Protocols_Sums![ProtocolFullTypeLocation.not_found_sums[1]] = unknown_protocols.Count;
                return unknown_protocols;
            }
            else
            {
                return null;
            }
        }
    }
}
