using System.Xml.Linq;
//using System.Text.RegularExpressions;
using BackupBlock;
using DrivesControl;
using Logging;


namespace Tracing
{
    class ProtocolsAnalysis(string period_month, FileInfo[] all_type_files) : ISimpleProtocolTypes
    {
        private readonly int month_value = MonthValues.Table[period_month];
        private List<List<int>?> protocol_type_numbers { get; set; } = [];
                                
        public Dictionary<string, int> Files_Sums { get; private set; } = new()
        {
            ["Всего"] = 0,
            ["ЕИАС"] = 0,
            ["Простые"] = 0,
            ["Пропущенные в этом месяце"] = 0,
            ["Неизвестные"] = 0
        };

        public List<string>? Missing_Protocols { get; private set; } = [];
        public List<string>? Unknown_Protocols { get; private set; } = [];  // по умолчанию нулл ??
        public List<FileInfo>? Result_Backup_Block
        {
            get => SelfCalculation();
        }

        private List<BackupItem> FindProtocols()
        {
            ProtocolScan eias = new(FileTypesPatterns.File_Patterns["EIAS"], month_value);
            BackupItem eias_backup = new(eias.Full_Pattern, all_type_files);

            ProtocolScan simple = new(FileTypesPatterns.File_Patterns["Simple"], month_value);
            BackupItem simple_backup = new(simple.Full_Pattern, all_type_files);

            return [eias_backup, simple_backup];
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
                List<int> current_protocols = protocol_type_numbers[type_index];

                if (current_protocols.Count > 2)
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

                if ((min_number < max_number) & ((max_number - 1) != min_number))
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

        private void AnalyseSimpleProtocols(List<FileInfo> files)
        {
            ProtocolTypeNumbers type_numbers = new(files);
            protocol_type_numbers = type_numbers.GetNumbers();
            ExtremeNumbers extreme_numbers = new(protocol_type_numbers);
            Dictionary<string, int> min_numbers = extreme_numbers.GetMinNumbers();

            CalculateMissingProtocols(min_numbers, extreme_numbers.GetMaxNumbers());
            // calc types protocols >>>
            //Files_Sums.Add("Уссурийск всего", protocol_type_numbers[0].Count + protocol_type_numbers[2].Count + protocol_type_numbers[4].Count);
            //Files_Sums.Add("Арсеньев всего", protocol_type_numbers[1].Count + protocol_type_numbers[3].Count + protocol_type_numbers[5].Count);

            for (int type_index = 0; type_index < ISimpleProtocolTypes.types_count; type_index++)
            {
                Files_Sums.Add(ISimpleProtocolTypes.protocol_types[type_index], protocol_type_numbers[type_index].Count);
            }
            // start find unknown protocols
            if (month_value != 1)
            {
                ProtocolScan previous_simple = new(FileTypesPatterns.File_Patterns["Simple"], month_value - 1);
                BackupItem previous_period_backup = new(previous_simple.Full_Pattern, all_type_files);

                if (previous_period_backup.Result_Files != null)
                {
                    ProtocolTypeNumbers previous_type_numbers = new(previous_period_backup.Result_Files);
                    List<List<int>> previous_protocol_type_numbers = previous_type_numbers.GetNumbers();
                    ExtremeNumbers previous_extreme_numbers = new(previous_protocol_type_numbers);

                    CalculateUnknownProtocols(previous_extreme_numbers.GetMaxNumbers(), min_numbers);
                }
            }
        }

        private List<FileInfo>? SelfCalculation()
        {
            List<BackupItem> backup_blocks = FindProtocols();
            // 0 - eias files; 1 - simple files
            if ((backup_blocks[0].Result_Files == null) & (backup_blocks[1].Result_Files == null))
            {
                return null;
            }
            else
            {
                Files_Sums["ЕИАС"] = backup_blocks[0].Files_Count;
                Files_Sums["Простые"] = backup_blocks[1].Files_Count;
                Files_Sums["Всего"] = backup_blocks[0].Files_Count + backup_blocks[1].Files_Count;

                if (backup_blocks[1].Result_Files != null)
                {
                    AnalyseSimpleProtocols(backup_blocks[1].Result_Files);
                }

                List<FileInfo> result_block = [];
                result_block.AddRange(backup_blocks[0].Result_Files);
                result_block.AddRange(backup_blocks[1].Result_Files);
                
                return result_block;
            }


                                 



        }



        

    }




    







}
