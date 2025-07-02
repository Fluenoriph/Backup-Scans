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


    struct SimpleProtocolsCalculation
    {
        public Dictionary<string, int> Sums { get; set; } = new()
        {
            //[ProtocolTypeLocation.others_sums[0]] = 0,

            //[ProtocolTypeLocation.others_sums[1]] = 0,
            //[ProtocolTypeLocation.others_sums[2]] = 0,

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

        public SimpleProtocolsCalculation() { }
    }

    // count control in backup directory
    class ProtocolsAnalysis(List<List<int>?> protocol_type_numbers) // только анализ простых
    {
        public Dictionary<string, int> Protocols_Sums
        {
            get
            {
                SimpleProtocolsCalculation protocol_calculation = new();
                // рассчет сумм типов протоколов и запись в словарь
                // >> рассчет всех типов и по району
                for (int type_index = 0; type_index < ProtocolTypeLocation.type_location_sums.Count; type_index++)
                {
                    string current_protocol_type = ProtocolTypeLocation.type_location_sums[type_index];
                    List<int>? current_protocol_numbers = protocol_type_numbers[type_index];

                    if (current_protocol_numbers != null)
                    {
                        protocol_calculation.Sums[current_protocol_type] = current_protocol_numbers.Count;
                    }
                }
                // test !!!
                // рассчет каждого типа всего
                for (int type_index = 0, calc_index = 0; type_index < ProtocolTypeLocation.type_full_sums.Count; type_index++)
                {
                    protocol_calculation.Sums[ProtocolTypeLocation.type_full_sums[type_index]] = protocol_calculation.Sums[ProtocolTypeLocation.type_location_sums[calc_index]] + protocol_calculation.Sums[ProtocolTypeLocation.type_location_sums[calc_index + 1]];
                    calc_index += 2;
                }
                // рассчет по району                            
                for (int city_index = 0, calc_index = 0; city_index < ProtocolTypeLocation.location_sums.Count; city_index++)
                {
                    protocol_calculation.Sums[ProtocolTypeLocation.location_sums[city_index]] = protocol_calculation.Sums[ProtocolTypeLocation.type_location_sums[calc_index]] + protocol_calculation.Sums[ProtocolTypeLocation.type_location_sums[calc_index + 2]] + protocol_calculation.Sums[ProtocolTypeLocation.type_location_sums[calc_index + 4]];
                    calc_index += 1;
                }

                return protocol_calculation.Sums;
            }
        }
    }


    class MissingProtocols(List<List<int>?> type_numbers) : ISimpleProtocolTypes
    {
        public List<int> Min_Numbers { get; private set; } = [];
        public List<string>? Missing_Protocols
        {
            get
            {
                MinimumNumbers extreme_min = new(type_numbers);
                Min_Numbers = extreme_min.Numbers;

                MaximumNumbers extreme_max = new(type_numbers);
                List<int> max_numbers = extreme_max.Numbers;

                List<string> missing_protocols = [];

                for (int type_index = 0; type_index < ISimpleProtocolTypes.types_count; type_index++)
                {
                    List<int>? current_protocols = type_numbers[type_index];

                    if (current_protocols != null)
                    {
                        if (current_protocols.Count >= 2)
                        {
                            List<int> range = CreateRange(Min_Numbers[type_index], max_numbers[type_index]);

                            IEnumerable<int> missing = range.Except(current_protocols);
                            List<int> missing_numbers = [.. missing];

                            foreach (int number in missing_numbers)
                            {
                                missing_protocols.Add($"{number}-{ISimpleProtocolTypes.protocol_types[type_index]}");
                            }
                        }
                    }
                }

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


    class UnknownProtocols(List<int> previous_max_numbers, List<int> current_min_numbers)
    {
        public List<string>? Unknown_Protocols
        {
            get
            {
                List<string> unknown_protocols = [];

                for (int type_index = 0; type_index < ISimpleProtocolTypes.types_count; type_index++)
                {
                    string current_type = ISimpleProtocolTypes.protocol_types[type_index];

                    int min_number = previous_max_numbers[type_index];
                    int max_number = current_min_numbers[type_index];

                    bool unknowns_ok = (min_number < max_number) & ((max_number - 1) != min_number);

                    if (unknowns_ok == true)
                    {
                        for (int start_num = min_number + 1; start_num < max_number; start_num++)     // если один протокол ??
                        {
                            unknown_protocols.Add($"{start_num}-{current_type}");
                        }
                    }
                }
                                
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
