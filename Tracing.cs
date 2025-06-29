using System.Xml.Linq;
//using System.Text.RegularExpressions;
using BackupBlock;
using DrivesControl;
using Logging;


namespace Tracing
{
    struct BackupProtocolsCalculation
    {
        public Dictionary<string, int> Files_Sums { get; set; } = new()
        {
            ["Всего"] = 0,

            ["ЕИАС"] = 0,
            ["Простые"] = 0,

            ["Пропущенные в этом месяце"] = 0,
            ["Неизвестные"] = 0,

            ["Физические факторы всего"] = 0,
            ["Физические факторы (Уссурийск)"] = 0,
            ["Физические факторы (Арсеньев)"] = 0,

            ["Радиационный контроль всего"] = 0,
            ["Радиационный контроль (Уссурийск)"] = 0,
            ["Радиационный контроль (Арсеньев)"] = 0,

            ["Измерения мебели всего"] = 0,
            ["Измерения мебели (Уссурийск)"] = 0,
            ["Измерения мебели (Арсеньев)"] = 0
        };

        public BackupProtocolsCalculation() { }
    }


    class ProtocolsAnalysis(string period_month, FileInfo[] all_type_files) : ISimpleProtocolTypes
    {
        private readonly int month_value = MonthValues.Table[period_month];
        private // type full names list's
        private List<List<int>?> Protocol_Type_Numbers { get; set; } = [];
        private BackupProtocolsCalculation protocol_calculation = new();

        public Dictionary<string, int> Files_Sums
        {
            get => protocol_calculation.Files_Sums;

            private set
            {
                _ = protocol_calculation.Files_Sums;
            }
        }
        
        public List<string>? Missing_Protocols { get; private set; } = [];
        public List<string>? Unknown_Protocols { get; private set; } = [];  // по умолчанию нулл ??
        public List<FileInfo>? Result_Backup_Block
        {
            get => SelfCalculation();
        }

        private List<FileInfo>? AnalyzeEiasProtocols()
        {
            ProtocolScanPattern eias = new(FileTypesPatterns.File_Patterns["EIAS"], month_value);
            BackupItem eias_backup = new(eias.Full_Pattern, all_type_files);
            
            if (eias_backup.Result_Files != null)
            {
                Files_Sums["ЕИАС"] = eias_backup.Result_Files.Count;

                return eias_backup.Result_Files;
            }
            else
            {
                Files_Sums["ЕИАС"] = 0;

                return null;
            }
        }

        private List<FileInfo>? AnalyzeSimpleProtocols()
        {
            ProtocolScanPattern simple = new(FileTypesPatterns.File_Patterns["Simple"], month_value);
            BackupItem simple_backup = new(simple.Full_Pattern, all_type_files);
            
            if (simple_backup.Result_Files != null)
            {
                Files_Sums["Простые"] = simple_backup.Result_Files.Count;
                ComputeSimpleProtocols(simple_backup.Result_Files);

                return simple_backup.Result_Files;
            }
            else
            {
                Files_Sums["Простые"] = 0;

                return null;
            }
        }

        private static List<int> CreateRange(int start, int end)
        {
            List<int> range = [];

            for (int i = start; i < end + 1; i++) range.Add(i);

            return range;
        }

        public void CalculateMissingProtocols(Dictionary<string, int> min_numbers, Dictionary<string, int> max_numbers)
        {
            List<string> missing_protocols = [];

            for (int type_index = 0; type_index < ISimpleProtocolTypes.types_count; type_index++)
            {
                string current_type = ISimpleProtocolTypes.protocol_types[type_index];
                List<int>? current_protocols = Protocol_Type_Numbers[type_index];   

                if (current_protocols != null)
                {
                    if (current_protocols.Count >= 2)
                    {
                        List<int> range = CreateRange(min_numbers[current_type], max_numbers[current_type]);

                        IEnumerable<int> missing = range.Except(current_protocols);
                        List<int> missing_numbers = [.. missing];

                        foreach (int number in missing_numbers)
                        {
                            missing_protocols.Add($"{number}-{current_type}");
                        }
                    }
                }
            }

            Files_Sums["Пропущенные в этом месяце"] = missing_protocols.Count;

            if (missing_protocols.Count > 0)
            {
                Missing_Protocols = missing_protocols;
            }
            else
            {
                Missing_Protocols = null;
            }
        }

        private void CalculateUnknownProtocols(Dictionary<string, int> previous_max_numbers, Dictionary<string, int> current_min_numbers)
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

            Files_Sums["Неизвестные"] = unknown_protocols.Count;

            if (unknown_protocols.Count > 0)
            {
                Unknown_Protocols = unknown_protocols;
            }
            else
            {
                Unknown_Protocols = null;
            }
        }

        private void ComputeSimpleProtocols(List<FileInfo> files)
        {
            ProtocolTypeNumbers type_numbers = new(files);
            Protocol_Type_Numbers = type_numbers.GetProtocolNumbers();
            ExtremeNumbers extreme_numbers = new(Protocol_Type_Numbers);
            Dictionary<string, int> min_numbers = extreme_numbers.GetMinNumbers();

            // рассчет сумм типов протоколов и запись в словарь
            for (int type_index = 0; type_index < ISimpleProtocolTypes.types_count; type_index++)
            {
                string current_protocol_type = ProtocolsFullNames.Table_Names[type_index];
                List<int>? current_protocol_numbers = Protocol_Type_Numbers[type_index];
                
                if (current_protocol_numbers != null)
                {
                    Files_Sums.Add(current_protocol_type, current_protocol_numbers.Count); 
                }
                else
                {
                    Files_Sums.Add(current_protocol_type, 0);
                }
            }

            Files_Sums.Add("Уссурийск всего", Files_Sums[ProtocolsFullNames.Table_Names[0]] + Files_Sums[ProtocolsFullNames.Table_Names[2]] + Files_Sums[ProtocolsFullNames.Table_Names[4]]);
            Files_Sums.Add("Арсеньев всего", Files_Sums[ProtocolsFullNames.Table_Names[1]] + Files_Sums[ProtocolsFullNames.Table_Names[3]] + Files_Sums[ProtocolsFullNames.Table_Names[5]]);

            CalculateMissingProtocols(min_numbers, extreme_numbers.GetMaxNumbers());

            // start find unknown protocols
            if (month_value != 1)
            {
                Console.WriteLine("\nРассчитываем пропущенные !");
                ProtocolScanPattern previous_simple = new(FileTypesPatterns.File_Patterns["Simple"], month_value - 1);
                BackupItem previous_period_backup = new(previous_simple.Full_Pattern, all_type_files);

                if (previous_period_backup.Result_Files != null)
                {
                    ProtocolTypeNumbers previous_type_numbers = new(previous_period_backup.Result_Files);
                    List<List<int>?> previous_protocol_type_numbers = previous_type_numbers.GetProtocolNumbers();
                    ExtremeNumbers previous_extreme_numbers = new(previous_protocol_type_numbers);

                    CalculateUnknownProtocols(previous_extreme_numbers.GetMaxNumbers(), min_numbers);
                }
            }
        }

        private List<FileInfo>? SelfCalculation()
        {
            List<FileInfo>? eias_files = AnalyzeEiasProtocols();
            List<FileInfo>? simple_files = AnalyzeSimpleProtocols();
            
            if ((eias_files != null) & (simple_files != null))
            {
                Files_Sums["Всего"] = Files_Sums["ЕИАС"] + Files_Sums["Простые"];

                List<FileInfo>? result_block = [];
                result_block.AddRange(eias_files);
                result_block.AddRange(simple_files);

                return result_block;
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




    







}
