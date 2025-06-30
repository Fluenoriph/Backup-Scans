using System.Collections.Generic;
using System.Xml.Linq;
using BackupBlock;
using DrivesControl;
using Logging;


namespace Tracing
{
    readonly struct ProtocolTypeLocation
    {
        public static List<string> others_sums = ["Всего", "ЕИАС", "Простые"]; // "Пропущенные в этом месяце", "Неизвестные"];

        public static List<string> location_sums = ["Уссурийск всего", "Арсеньев всего"];

        public static List<string> type_location_sums = ["Физические факторы (Уссурийск)", "Физические факторы (Арсеньев)",
                                                           "Радиационный контроль (Уссурийск)", "Радиационный контроль (Арсеньев)",
                                                           "Измерения мебели (Уссурийск)", "Измерения мебели (Арсеньев)"];

        public static List<string> type_full_sums = ["Физические факторы всего", "Радиационный контроль всего", "Измерения мебели всего"];
                
        public ProtocolTypeLocation() { }
    }


    struct BackupProtocolsCalculation
    {
        public Dictionary<string, int> Files_Sums { get; set; } = new()
        {
            [ProtocolTypeLocation.others_sums[0]] = 0,

            [ProtocolTypeLocation.others_sums[1]] = 0,
            [ProtocolTypeLocation.others_sums[2]] = 0,

            //[ProtocolTypeLocation.others_sums[3]] = 0,
            //[ProtocolTypeLocation.others_sums[4]] = 0,

            [ProtocolTypeLocation.location_sums[0]] = 0,
            [ProtocolTypeLocation.location_sums[1]] = 0,

            [ProtocolTypeLocation.type_full_sums[0]] = 0,
            [ProtocolTypeLocation.type_location_sums[0]] = 0,
            [ProtocolTypeLocation.type_location_sums[1]] = 0,

            [ProtocolTypeLocation.type_full_sums[1]] = 0,
            [ProtocolTypeLocation.type_location_sums[2]] = 0,
            [ProtocolTypeLocation.type_location_sums[3]] = 0,

            [ProtocolTypeLocation.type_full_sums[2]] = 0,
            [ProtocolTypeLocation.type_location_sums[4]] = 0,
            [ProtocolTypeLocation.type_location_sums[5]] = 0
        };

        public BackupProtocolsCalculation() { }
    }

    // count control in backup directory
    class ProtocolsAnalysis(int month_value, FileInfo[] all_type_files) 
    {
        private BackupProtocolsCalculation protocol_calculation = new();
        public List<List<int>?> Protocol_Type_Numbers { get; set; } = [];
        public Dictionary<string, int> Files_Sums
        {
            get => protocol_calculation.Files_Sums;

            private set
            {
                _ = protocol_calculation.Files_Sums;
            }
        }   
        public List<FileInfo>? Result_Backup_Block
        {
            get => GetResultBlock();
        }

        private List<FileInfo>? AnalyzeEIASProtocols()
        {
            ProtocolScanPattern eias = new(FileTypesPatterns.File_Patterns["EIAS"], month_value);
            BackupItem eias_backup = new(eias.Full_Pattern, all_type_files);
            
            if (eias_backup.Result_Files != null)
            {
                Files_Sums[ProtocolTypeLocation.others_sums[1]] = eias_backup.Result_Files.Count;

                return eias_backup.Result_Files;
            }
            else
            {
                Files_Sums[ProtocolTypeLocation.others_sums[1]] = 0;

                return null;
            }
        }

        private List<FileInfo>? AnalyzeSimpleProtocols()
        {
            ProtocolScanPattern simple = new(FileTypesPatterns.File_Patterns["Simple"], month_value);
            BackupItem simple_backup = new(simple.Full_Pattern, all_type_files);
            
            if (simple_backup.Result_Files != null)
            {
                Files_Sums[ProtocolTypeLocation.others_sums[2]] = simple_backup.Result_Files.Count;
                ComputeTypeSums(simple_backup.Result_Files);

                return simple_backup.Result_Files;
            }
            else
            {
                Files_Sums[ProtocolTypeLocation.others_sums[2]] = 0;

                return null;
            }
        }
        
        private void ComputeTypeSums(List<FileInfo> files)
        {
            ProtocolTypeNumbers type_numbers = new(files);
            Protocol_Type_Numbers = type_numbers.GetProtocolNumbers();
                        
            // рассчет сумм типов протоколов и запись в словарь
            // >> рассчет всех типов и по району
            for (int type_index = 0; type_index < ProtocolTypeLocation.type_location_sums.Count; type_index++)
            {
                string current_protocol_type = ProtocolTypeLocation.type_location_sums[type_index];
                List<int>? current_protocol_numbers = Protocol_Type_Numbers[type_index];
                
                if (current_protocol_numbers != null)
                {
                    Files_Sums[current_protocol_type] = current_protocol_numbers.Count; 
                }
                else
                {
                    Files_Sums[current_protocol_type] = 0;
                }
            }
            // test !!!
            // рассчет каждого типа всего
            for (int type_index = 0, calc_index = 0; type_index < ProtocolTypeLocation.type_full_sums.Count; type_index++)
            {
                Files_Sums[ProtocolTypeLocation.type_full_sums[type_index]] = Files_Sums[ProtocolTypeLocation.type_location_sums[calc_index]] + Files_Sums[ProtocolTypeLocation.type_location_sums[calc_index + 1]];
                calc_index += 2;
            }
            // рассчет по району                            
            for (int city_index = 0, calc_index = 0; city_index < ProtocolTypeLocation.location_sums.Count; city_index++)
            {
                Files_Sums[ProtocolTypeLocation.location_sums[city_index]] = Files_Sums[ProtocolTypeLocation.type_location_sums[calc_index]] + Files_Sums[ProtocolTypeLocation.type_location_sums[calc_index + 2]] + Files_Sums[ProtocolTypeLocation.type_location_sums[calc_index + 4]];
                calc_index += 1;
            }   
        }

        private List<FileInfo>? GetResultBlock()
        {
            List<FileInfo>? eias_files = AnalyzeEIASProtocols();
            List<FileInfo>? simple_files = AnalyzeSimpleProtocols();
            
            if ((eias_files != null) & (simple_files != null))
            {
                Files_Sums[ProtocolTypeLocation.others_sums[0]] = Files_Sums[ProtocolTypeLocation.others_sums[1]] + Files_Sums[ProtocolTypeLocation.others_sums[2]];

                IEnumerable<FileInfo> result_block = eias_files.Concat(simple_files);
                // disable null pragma ??
                List<FileInfo> result = [.. result_block];

                return result;
            }
            else if ((simple_files != null) & (eias_files == null))
            {
                return simple_files;
            }
            else if ((eias_files != null) & (simple_files == null))
            {
                return eias_files;
            }
            else
            {
                return null;
            }
        }
    }


    class MissingProtocols(List<List<int>?> type_numbers) : ISimpleProtocolTypes
    {
        public Dictionary<string, int> Min_Numbers { get; private set; } = [];
        public List<string>? Missing_Protocols
        {
            get => CalculateMissingProtocols();
        }

        private static List<int> CreateRange(int start, int end)
        {
            List<int> range = [];

            for (int i = start; i < end + 1; i++) range.Add(i);

            return range;
        }

        public List<string>? CalculateMissingProtocols()
        {
            ExtremeNumbers extreme_numbers = new(type_numbers);
            Min_Numbers = extreme_numbers.GetMinNumbers();
            Dictionary<string, int> max_numbers = extreme_numbers.GetMaxNumbers();

            List<string> missing_protocols = [];

            for (int type_index = 0; type_index < ISimpleProtocolTypes.types_count; type_index++)
            {
                string current_type = ISimpleProtocolTypes.protocol_types[type_index];
                List<int>? current_protocols = type_numbers[type_index];

                if (current_protocols != null)
                {
                    if (current_protocols.Count >= 2)
                    {
                        List<int> range = CreateRange(Min_Numbers[current_type], max_numbers[current_type]);

                        IEnumerable<int> missing = range.Except(current_protocols);
                        List<int> missing_numbers = [.. missing];

                        foreach (int number in missing_numbers)
                        {
                            missing_protocols.Add($"{number}-{current_type}");
                        }
                    }
                }
            }

            //Files_Sums["Пропущенные в этом месяце"] = missing_protocols.Count;  // test !!

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



        /*private void CalculateUnknownProtocols(Dictionary<string, int> previous_max_numbers, Dictionary<string, int> current_min_numbers)
        {
            List<string> unknown_protocols = [];

            for (int type_index = 0; type_index < ISimpleProtocolTypes.types_count; type_index++)
            {
                string current_type = ISimpleProtocolTypes.protocol_types[type_index];
                int min_number = previous_max_numbers[current_type];
                int max_number = current_min_numbers[current_type];

                bool unknowns_ok = (min_number < max_number) & ((max_number - 1) != min_number);

                if (unknowns_ok == true)
                {
                    for (int start_num = min_number + 1; start_num < max_number; start_num++)     // если один протокол ??
                    {
                        unknown_protocols.Add($"{start_num}-{current_type}");
                    }
                }
            }

            Files_Sums["Неизвестные"] = unknown_protocols.Count; // test !!

            if (unknown_protocols.Count > 0)
            {
                Unknown_Protocols = unknown_protocols;
            }
            else
            {
                Unknown_Protocols = null;
            }
        }



        if (month_value != 1)
            {         // отдельный метод !!
                Console.WriteLine("\nРассчитываем пропущенные !\n");
                ProtocolScanPattern previous_simple = new(FileTypesPatterns.File_Patterns["Simple"], month_value - 1);
                BackupItem previous_period_backup = new(previous_simple.Full_Pattern, all_type_files);

                if (previous_period_backup.Result_Files != null)
                {
                    ProtocolTypeNumbers previous_type_numbers = new(previous_period_backup.Result_Files);
                    List<List<int>?> previous_protocol_type_numbers = previous_type_numbers.GetProtocolNumbers();
                    ExtremeNumbers previous_extreme_numbers = new(previous_protocol_type_numbers);

                    CalculateUnknownProtocols(previous_extreme_numbers.GetMaxNumbers(), min_numbers);
                }
            }*/
    










}
