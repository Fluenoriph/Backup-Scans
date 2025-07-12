using System.Text.RegularExpressions;
using BackupBlock;
using DrivesControl;
using Logging;
using Tracing;


const string line = "- - - - - - - - - - - - - - -";
Console.WriteLine($"{line}\n* Nebulium 1.0 * / Test 2025\n{line}\n");

XMLConfig drives_config = new();

string source_dir = drives_config.Drives[0].Directory;
//string destination_dir = drives_config.Drives[1].Directory;

Console.WriteLine("\nВыберите, что нужно копировать:\n[1] - Сканы протоколов");
string backup_items_type = Console.ReadLine();   // отдельный класс для проверки пустой строки

switch (backup_items_type)
{ 
    
    case "1":
        PdfFiles pdf_files = new(FileTypesPatterns.file_types["PDF"], source_dir);

        if (pdf_files.Found_Status)
        {
            Dictionary<string, int> all_protocols_sums = new()    // class field ???
            {
                [ProtocolFullTypeLocation.others_sums[0]] = 0, // всего
                [ProtocolFullTypeLocation.others_sums[1]] = 0, // еиас
                [ProtocolFullTypeLocation.others_sums[2]] = 0,   // простые
            };

            Console.WriteLine("\nPDF IS OK !!!");
            Console.WriteLine("\nВведите месяц, за который выполнить копирование:");
            // должна быть проверка правильности ввода месяца
            string current_period = Console.ReadLine();
            int current_month_value = MonthValues.Table[current_period];
            ///////////
            ProtocolScanPattern protocol_pattern = new();
            ProtocolScanPattern.Month_Value = current_month_value;

            // инициализация сканов
            List<List<FileInfo>?> backup_block = [];

            for (int protocol_type_index = 0; protocol_type_index < FileTypesPatterns.protocol_file_type.Count; protocol_type_index++)
            {
                Regex type_pattern = protocol_pattern.CreatePattern(FileTypesPatterns.file_patterns[FileTypesPatterns.protocol_file_type[protocol_type_index]]);
                List<FileInfo>? files = pdf_files.GrabMatchedFiles(type_pattern);

                backup_block.AddRange(files);
                
                if (files is not null)
                {
                    all_protocols_sums[ProtocolFullTypeLocation.others_sums[protocol_type_index + 1]] = files.Count;
                    all_protocols_sums[ProtocolFullTypeLocation.others_sums[0]] += files.Count;
                }
            }
            //////------------------------------------------
            // это отдельное вообще
            if (backup_block[1] is not null)
            {
                ProtocolTypeNumbers current_type_numbers = new(backup_block[1]);
                //var numbers = type_numbers.Numbers;
                ////
                ProtocolsAnalysis protocols_analysis = new(current_type_numbers.Numbers);

                foreach (var item in protocols_analysis.Simple_Protocols_Sums)
                {
                    all_protocols_sums.Add(item.Key, item.Value);
                }
                //////
                MissingProtocols missing_protocols = new(current_type_numbers.Numbers);

                if (missing_protocols.Missing_Protocols is not null)
                {
                    all_protocols_sums.Add(ProtocolFullTypeLocation.not_found_sums[0], missing_protocols.Missing_Protocols.Count);
                }
                ////// 
                if (current_month_value is not 1)
                {
                    ProtocolScanPattern.Month_Value = current_month_value - 1;
                    Regex previous_period_pattern = protocol_pattern.CreatePattern(FileTypesPatterns.file_patterns[FileTypesPatterns.protocol_file_type[1]]);
                    List<FileInfo>? previous_period_files = pdf_files.GrabMatchedFiles(previous_period_pattern);
                    ///////
                    if (previous_period_files is not null)
                    {
                        ProtocolTypeNumbers previous_type_numbers = new(previous_period_files);

                        MaximumNumbers previous_max = new(previous_type_numbers.Numbers);

                        UnknownProtocols unknown = new(previous_max.Numbers, missing_protocols.Min_Numbers);
                        ///////////
                        if (unknown.Unknown_Protocols is not null)
                        {
                            all_protocols_sums.Add(ProtocolFullTypeLocation.not_found_sums[1], unknown.Unknown_Protocols.Count);
                        }
                    }

                }

                
            }




            foreach (var count in all_protocols_sums)
            {
                Console.WriteLine($"{count.Key} - {count.Value}");
            }

            
            
            
            





        }
        else
        {
            Console.WriteLine("\nPDF NOT FOUND !!"); // return false
        }


        break;
        

}

    























// C:\Users\Asus machine\Desktop\Files\сканы
// C:\Users\Asus machine\Desktop\Files\result_test


