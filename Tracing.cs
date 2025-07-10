using System.Collections.Generic;
using System.Xml.Linq;
using BackupBlock;
using DrivesControl;
using Logging;


namespace Tracing
{
    readonly struct ProtocolFullTypeLocation
    {
        public static List<string> others_sums = ["Всего", "ЕИАС", "Простые"];

        public static List<string> not_found_sums = ["Пропущенные в этом месяце", "Неизвестные"];   //создать перед применением ??

        public static List<string> location_sums = ["Уссурийск всего", "Арсеньев всего"];

        public static List<string> type_location_sums = ["Физические факторы (Уссурийск)", "Физические факторы (Арсеньев)",
                                                           "Радиационный контроль (Уссурийск)", "Радиационный контроль (Арсеньев)",
                                                           "Измерения мебели (Уссурийск)", "Измерения мебели (Арсеньев)"];

        public static List<string> type_full_sums = ["Физические факторы всего", "Радиационный контроль всего", "Измерения мебели всего"];
                
        public ProtocolFullTypeLocation() { }
    }


    struct ProtocolsCalculation
    {
        public Dictionary<string, int> Full_Sums { get; set; } = new()
        {
            [ProtocolFullTypeLocation.others_sums[0]] = 0,
            [ProtocolFullTypeLocation.others_sums[1]] = 0,
            [ProtocolFullTypeLocation.others_sums[2]] = 0,
        };
                
        public Dictionary<string, int> Unknown_Sums { get; set; } = new()
        {
            [ProtocolFullTypeLocation.others_sums[3]] = 0,
            [ProtocolFullTypeLocation.others_sums[4]] = 0,
        };

        public ProtocolsCalculation() { }
    }


    class FileTransfer(List<FileInfo> backup_files)
    {
        private readonly List<FileInfo> files = backup_files;

        public void CopyBackupFiles(string target_directory)
        {



        }


    }




    /*class BackupProcess(int month_value, FileInfo[] pdf_files)
    {
        private List<List<FileInfo>?>? Backup_Files         // else null, then stop !!
        {
            get
            {   // create a backup list [EIAS, Simple]
                List<List<FileInfo>?> backup_files = [];

                foreach (string protocol_file_type in FileTypesPatterns.protocol_file_type)
                {
                    ProtocolScanGrabbing protocol_file_type_capture = new(protocol_file_type, month_value, pdf_files);
                    backup_files.AddRange(protocol_file_type_capture.Files);
                }

                if ((backup_files[0] == null) && (backup_files[1] == null))
                {
                    return null; // ???
                }
                else
                {
                    return backup_files;
                }  
            }
        }*/






        
        // объект транспортировщик в бэкап ??


        



         

    




    // count control in backup Directory
    class ProtocolsAnalysis // только анализ простых
    {
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

        public ProtocolsAnalysis(List<List<int>?> protocol_type_numbers)
        {
            // рассчет сумм типов протоколов и запись в словарь
            // >> рассчет всех типов и по району
            for (int type_index = 0; type_index < ProtocolFullTypeLocation.type_location_sums.Count; type_index++)
            {
                string current_protocol_type = ProtocolFullTypeLocation.type_location_sums[type_index];
                List<int>? current_protocol_numbers = protocol_type_numbers[type_index];

                if (current_protocol_numbers != null)
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
    }


    class MissingProtocols 
    {
        private readonly List<string> missing_protocols = [];
        public List<int> Min_Numbers { get; private set; } = [];
        
        public MissingProtocols(List<List<int>?> type_numbers)
        {
            MinimumNumbers extreme_min = new(type_numbers);
            Min_Numbers = extreme_min.Numbers;

            MaximumNumbers extreme_max = new(type_numbers);
            List<int> max_numbers = extreme_max.Numbers;

            for (int type_index = 0; type_index < SimpleProtocolTypes.types_count; type_index++)
            {
                List<int>? current_protocols = type_numbers[type_index];

                if ((current_protocols is not null) && (current_protocols.Count >= 2))
                {
                    List<int> range = CreateRange(Min_Numbers[type_index], max_numbers[type_index]);

                    IEnumerable<int> missing = range.Except(current_protocols);
                    List<int> missing_numbers = [.. missing];

                    foreach (int number in missing_numbers)
                    {
                        missing_protocols.Add($"{number}-{SimpleProtocolTypes.protocol_types[type_index]}");
                    }
                }
            }
        }

        public List<string>? Missing_Protocols
        {
            get
            {
                if (missing_protocols.Count > 0)
                {
                    return missing_protocols;
                }
                else
                {
                    return null;
                }
            }
        }
            
        private static List<int> CreateRange(int start, int end)
        {
            List<int> range = [];

            for (int i = start; i < end + 1; i++) range.Add(i);

            return range;
        }
    }


    class UnknownProtocols
    {
        private readonly List<string> unknown_protocols = [];

        public UnknownProtocols(List<int> previous_max_numbers, List<int> current_min_numbers)
        {
            for (int type_index = 0; type_index < SimpleProtocolTypes.types_count; type_index++)
            {
                string current_type = SimpleProtocolTypes.protocol_types[type_index];

                int min_number = previous_max_numbers[type_index];
                int max_number = current_min_numbers[type_index];

                bool unknowns_ok = (min_number < max_number) && ((max_number - 1) != min_number);

                if (unknowns_ok)
                {
                    for (int start_num = min_number + 1; start_num < max_number; start_num++)     // если один протокол ??
                    {
                        unknown_protocols.Add($"{start_num}-{current_type}");
                    }
                }
            }
        }

        public List<string>? Unknown_Protocols
        {
            get
            {                
                if (unknown_protocols.Count > 0)
                {
                    return unknown_protocols;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
