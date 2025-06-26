using System.Xml.Linq;
//using System.Text.RegularExpressions;
using BackupBlock;
using DrivesControl;
using Logging;


namespace Tracing
{
    class ProtocolsAnalysis(string period_month, FileInfo[] files_of_type) : ISimpleProtocolTypes
    {
        private List<List<int>> protocol_type_numbers = [];
        //public Dictionary<string, int> Protocol_Types_Sums { get; set; } = [];
        public List<FileInfo> Result_Backup_Block { get; set; } = [];

        public Dictionary<string, int> Files_Sums { get; set; } = new()
        {
            ["Всего"] = 0,
            ["ЕИАС"] = 0,
            ["Простые"] = 0,
            ["Уссурийск всего"] = 0,
            ["Арсеньев всего"] = 0,
            // ф
            //фа
            ["Пропущенные в этом месяце"] = 0,
            ["Неизвестные"] = 0
        };

        public List<string>? Missing_Protocols { get; private set; } = [];
        public List<string>? Unknown_Protocols { get; private set; } = [];

        private static List<int> CreateRange(int start, int end)
        {
            List<int> range = [];

            for (int i = start; i < end + 1; i++) range.Add(i);

            return range;
        }

        public List<string> GetMissingProtocols(Dictionary<string, int> min_numbers, Dictionary<string, int> max_numbers)
        {
            for (int type_index = 0; type_index < ISimpleProtocolTypes.types_count; type_index++)
            {
                string current_type = ISimpleProtocolTypes.protocol_types[type_index];
                List<int> current_protocols = protocol_type_numbers[type_index];

                if (current_protocols.Count > 2)
                {
                    List<int> range = CreateRange(min_numbers[current_type], max_numbers[current_type]);

                    List<int> missing_numbers = (List<int>)range.Except(current_protocols);

                    foreach (int number in missing_numbers)
                    {
                        Missing_Protocols.Add($"{number}-{current_type}");
                    }
                }
            }

            Files_Sums.Add("Пропущенные в этом месяце", missing_protocols.Count);

            if (missing_protocols.Count > 0)
            {
                return missing_protocols;
            }
            else
            {
                return null;
            }
            
        }

        private List<string>? GetUnknownProtocols()
        {
            if (max_numbers_previous_period != null)
            {
                List<string> unknown_protocols = [];
                for (int type_index = 0; type_index < types_count; type_index++)
                {
                    string current_type = protocol_types[type_index];
                    int current_min_number = min_numbers[current_type];
                    int current_max_number = max_numbers_previous_period[current_type];

                    if ((current_max_number < current_min_number) & ((current_min_number - 1) != current_max_number))
                    {
                        for (int start_num = current_max_number + 1; start_num < current_min_number; start_num++)     // если один протокол ??
                        {
                            unknown_protocols.Add($"{start_num}-{current_type}");
                        }
                    }
                }

                return unknown_protocols;
            }
            else
            {
                return null;
            }
        }



        private void SelfCalculation()
        {
            int month_value = MonthValues.Table[period_month];

            ProtocolScan eias = new(FileTypesPatterns.File_Patterns["EIAS"], month_value);
            BackupItem eias_backup_block = new(eias.Full_Pattern, files_of_type);
            Files_Sums.Add("ЕИАС", eias_backup_block.Files_Count);

            ProtocolScan simple = new(FileTypesPatterns.File_Patterns["Simple"], month_value);
            BackupItem simple_backup_block = new(simple.Full_Pattern, files_of_type);
            Files_Sums.Add("Простые", simple_backup_block.Files_Count);

            Files_Sums.Add("Всего", eias_backup_block.Files_Count + simple_backup_block.Files_Count);

            if (simple_backup_block.Result_Files != null)
            {
                ProtocolTypeNumbers type_numbers = new(simple_backup_block.Result_Files);
                List<List<int>> protocol_type_numbers = type_numbers.GetNumbers();
                ExtremeNumbers extreme_numbers = new(protocol_type_numbers);
                Dictionary<string, int> min_numbers = extreme_numbers.GetMinNumbers();

                Missing_Protocols = GetMissingProtocols(min_numbers, extreme_numbers.GetMaxNumbers());

                if (month_value != 1)
                {
                    ProtocolScan simple_previous = new(FileTypesPatterns.File_Patterns["Simple"], month_value - 1);
                    BackupItem previous_period_backup_block = new(simple_previous.Full_Pattern, files_of_type);




                }
                



            }
            



        }



        

    }




    







}
