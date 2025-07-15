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

        public static List<string> not_found_sums = ["Пропущенные в этом месяце", "Неизвестные"];   

        public static List<string> location_sums = ["Уссурийск всего", "Арсеньев всего"];

        public static List<string> type_location_sums = ["Физические факторы (Уссурийск)", "Физические факторы (Арсеньев)",
                                                           "Радиационный контроль (Уссурийск)", "Радиационный контроль (Арсеньев)",
                                                           "Измерения мебели (Уссурийск)", "Измерения мебели (Арсеньев)"];

        public static List<string> type_full_sums = ["Физические факторы всего", "Радиационный контроль всего", "Измерения мебели всего"];
                
        public ProtocolFullTypeLocation() { }
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
    class ProtocolsAnalysis 
    {
        private protected List<int> current_period_min_numbers = [];
        
        private readonly Func<int, int, List<int>> CreateRange = (start, end) =>
        {
            List<int> range = [];
            for (int i = start; i < end + 1; i++) range.Add(i);
            return range;
        };
                
        public ProtocolsAnalysis(List<FileInfo> captured_simple_files)
        {
            ProtocolTypeNumbers obj_protocol_type_numbers = new(captured_simple_files);

            CalculateTypeSums(obj_protocol_type_numbers.Numbers);
            Missing_Protocols = ComputeMissingProtocols(obj_protocol_type_numbers.Numbers);
        }

        public Dictionary<string, int> Simple_Protocols_Sums { get; set; } = new()
        {
            [ProtocolFullTypeLocation.location_sums[0]] = 0,
            [ProtocolFullTypeLocation.location_sums[1]] = 0,

            [ProtocolFullTypeLocation.type_full_sums[0]] = 0,
            [ProtocolFullTypeLocation.type_location_sums[0]] = 0,
            [ProtocolFullTypeLocation.type_location_sums[1]] = 0,

            [ProtocolFullTypeLocation.type_full_sums[1]] = 0,
            [ProtocolFullTypeLocation.type_location_sums[2]] = 0,
            [ProtocolFullTypeLocation.type_location_sums[3]] = 0,

            [ProtocolFullTypeLocation.type_full_sums[2]] = 0,
            [ProtocolFullTypeLocation.type_location_sums[4]] = 0,
            [ProtocolFullTypeLocation.type_location_sums[5]] = 0
        };

        public List<string>? Missing_Protocols { get; set; }
        
        private void CalculateTypeSums(List<List<int>?> protocol_type_numbers)
        {
            // рассчет сумм типов протоколов и запись в словарь
            // >> рассчет всех типов и по району
            for (int type_index = 0; type_index < ProtocolFullTypeLocation.type_location_sums.Count; type_index++)
            {
                string current_protocol_type = ProtocolFullTypeLocation.type_location_sums[type_index];
                List<int>? current_protocol_numbers = protocol_type_numbers[type_index];

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
                
        private List<string>? ComputeMissingProtocols(List<List<int>?> protocol_type_numbers)
        {
            MinimumNumbers extreme_min = new(protocol_type_numbers);
            current_period_min_numbers = extreme_min.Numbers;

            MaximumNumbers extreme_max = new(protocol_type_numbers);
            List<int> max_numbers = extreme_max.Numbers;

            List<string> missing_protocols = [];

            for (int type_index = 0; type_index < SimpleProtocolTypes.types_count; type_index++)
            {
                List<int>? current_protocols = protocol_type_numbers[type_index];

                if ((current_protocols is not null) && (current_protocols.Count >= 2))
                {
                    List<int> range = CreateRange(current_period_min_numbers[type_index], max_numbers[type_index]);

                    IEnumerable<int> missing = range.Except(current_protocols);
                    List<int> missing_numbers = [.. missing];

                    foreach (int number in missing_numbers)
                    {
                        missing_protocols.Add($"{number}-{SimpleProtocolTypes.protocol_types[type_index]}");
                    }
                }
            }

            if (missing_protocols.Count is not 0)
            {
                Simple_Protocols_Sums.Add(ProtocolFullTypeLocation.not_found_sums[0], missing_protocols.Count);
                return missing_protocols;
            }
            else
            {
                return null;
            }
        }
    }


    class AnalysWithUnknownProtocols : ProtocolsAnalysis
    {
        public AnalysWithUnknownProtocols(List<FileInfo> captured_simple_files, int previous_month_value, PdfFiles source_files) : base(captured_simple_files)
        {
            ProtocolScanPattern self_obj_protocol_pattern = new(previous_month_value);

            Regex pattern = self_obj_protocol_pattern.CreatePattern(FileTypesPatterns.file_patterns[FileTypesPatterns.protocol_file_type[1]]);
            List<FileInfo>? files = source_files.GrabMatchedFiles(pattern);

            if (files is not null)
            {
                ProtocolTypeNumbers previous_type_numbers = new(files);
                
                MaximumNumbers previous_max = new(previous_type_numbers.Numbers);

                Unknown_Protocols = ComputeUnknownProtocols(previous_max.Numbers, current_period_min_numbers);
            }
            else
            {
                Unknown_Protocols = null;
            }
        }

        public List<string>? Unknown_Protocols { get; set; }

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
                Simple_Protocols_Sums.Add(ProtocolFullTypeLocation.not_found_sums[1], unknown_protocols.Count);
                return unknown_protocols;
            }
            else
            {
                return null;
            }
        }
    }


    class BackupFiles
    {
        private readonly List<List<FileInfo>?> backup_block = [];
                               
        // logger 
        // log out
        // log write


        public BackupFiles(int month_value, PdfFiles source_files)
        {
            ProtocolScanPattern self_obj_protocol_pattern = new(month_value);

            for (int protocol_type_index = 0; protocol_type_index < FileTypesPatterns.protocol_file_type.Count; protocol_type_index++)
            {       
                Regex type_pattern = self_obj_protocol_pattern.CreatePattern(FileTypesPatterns.file_patterns[FileTypesPatterns.protocol_file_type[protocol_type_index]]);
                List<FileInfo>? files = source_files.GrabMatchedFiles(type_pattern);
                
                backup_block.AddRange(files);

                if (files is not null)
                {
                    All_Protocols_Sums[ProtocolFullTypeLocation.others_sums[0]] += files.Count;
                    All_Protocols_Sums[ProtocolFullTypeLocation.others_sums[protocol_type_index + 1]] = files.Count;
                }
            }
        }

        public List<List<FileInfo>?>? Backup_Block
        {
            get
            {
                if ((backup_block[0] is null) && (backup_block[1] is null))
                {
                    return null;
                }
                else
                {
                    return backup_block;
                }
            }
        }

        public Dictionary<string, int> All_Protocols_Sums { get; } = new()
        {
            [ProtocolFullTypeLocation.others_sums[0]] = 0, // всего
            [ProtocolFullTypeLocation.others_sums[1]] = 0, // еиас
            [ProtocolFullTypeLocation.others_sums[2]] = 0,   // простые
        };
    }
}
