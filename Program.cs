using System.Text.RegularExpressions;
using BackupBlock;
using DrivesControl;
using Logging;
using Tracing;


const string line = "- - - - - - - - - - - - - - -";
Console.WriteLine($"{line}\n* Backup PDF v.1.0 * / Test 2025\n{line}\n");

XMLConfig self_obj_drives_config = new();
string source_directory = self_obj_drives_config.Drives[0].Directory;
//string destination_dir = drives_config.Drives[1].Directory;

Console.WriteLine("\nВыберите, что нужно копировать:\n[1] - Сканы протоколов");
string backup_items_type = "1";//Console.ReadLine();   // отдельный класс для проверки пустой строки

switch (backup_items_type)
{
    case "1":
        // while ??

        Console.WriteLine("\nВведите месяц, за который выполнить копирование:");   // если год, это статическая структура даты
        // должна быть проверка правильности ввода месяца
        string current_period = Console.ReadLine();

        PdfFiles self_obj_pdf_files = new(source_directory);

        if (MonthValues.Month_Names.Contains(current_period))
        {
            Console.WriteLine("\nЗапущено копирование в месяц !\n");

            BackupProcess self_obj_month_backup = new(self_obj_pdf_files, current_period);
        }
        else if (current_period is "год")
        {
            Console.WriteLine("\nЗапущено копирование за год !");
        }
        else
        {
            Console.WriteLine("\nОшибка ввода периода !");
            System.Environment.Exit(0);
        }
        break;
}




               






                /*if (self_obj_backup.Backup_Block is not null) // return false
                {
                    Console.WriteLine($"\nЗа {current_period} найдено:\n{line}");
                    // logger
                    foreach (var item in self_obj_backup.All_Protocols_Sums)
                    {
                        Console.WriteLine($"{item.Key}: {item.Value}");
                    }

                    foreach (var files in self_obj_backup.Backup_Block)
                    {
                        if (files is not null)
                        {
                            foreach (var file in files)
                            {
                                Console.WriteLine(file.Name);
                            }
                        }  
                    }




                    //-----------------------------------------------------------
                    if (self_obj_backup.Backup_Block[1] is not null)
                    {
                        if (current_month_value is not 1)
                        {
                            AnalysWithUnknownProtocols self_obj_analysis_with_unknowns = new(self_obj_backup.Backup_Block[1], current_month_value - 1, self_obj_pdf_files);

                            foreach (var item in self_obj_analysis_with_unknowns.Simple_Protocols_Sums)
                            {
                                Console.WriteLine($"{item.Key}: {item.Value}");
                            }
                            // show missing scans
                            if (self_obj_analysis_with_unknowns.Missed_Protocols is not null)
                            {
                                Console.WriteLine("\nПропущены >>>");
                                foreach (var miss_file in self_obj_analysis_with_unknowns.Missed_Protocols)
                                {
                                    Console.WriteLine(miss_file);
                                }
                            }
                            // snow unknowns scans
                            if (self_obj_analysis_with_unknowns.Unknown_Protocols is not null)
                            {
                                Console.WriteLine("\nНеизвестные >>>");
                                foreach (var unknown_file in self_obj_analysis_with_unknowns.Unknown_Protocols)
                                {  
                                    Console.WriteLine(unknown_file); 
                                }
                            }

                        }
                        else
                        {   // отдельный метод
                            ProtocolsAnalysis self_obj_analysis = new(self_obj_backup.Backup_Block[1]);

                            foreach (var item in self_obj_analysis.Simple_Protocols_Sums)
                            {
                                Console.WriteLine($"{item.Key}: {item.Value}");
                            }

                            // show missing in january ----------------------------
                            if (self_obj_analysis.Missed_Protocols is not null)
                            {
                                Console.WriteLine("\nПропущены >>>");
                                foreach (var miss_file in self_obj_analysis.Missed_Protocols)
                                {
                                    Console.WriteLine(miss_file);
                                }
                            }
                        }
                    }

                }
                else
                {
                    Console.WriteLine($"\nЗа {current_period} сканов не найдено !");
                }*/
                  
    

class BackupProcess
{
    //private readonly List<List<FileInfo>?>? backup_item;  // словарь пропущенных ???
    private Dictionary<string, int>? Sums;
    private int month_value;
    private readonly BackupFilesMonth? self_obj_backup_item;  // required ??? googling !!

    public bool Search_Status { get; private set; } // no ??

    public BackupProcess(PdfFiles source_files, string month)
    {
        if (source_files.Files is not null)
        {
            month_value = MonthValues.Table[month];
            self_obj_backup_item = new(month_value, source_files);
                                   
            if (self_obj_backup_item.Files is not null)
            {
                Search_Status = true;
                                
                CalcAllSums();

                if (self_obj_backup_item.Files[1] is not null)
                {
                    SearchNoneProtocols();
                }

                // show log
                foreach (var item in Sums)
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }


            }
            else
            {
                Search_Status = false;
                Console.WriteLine($"\nЗа {month} сканов не найдено ! ***");  // out 
            }
        }
        else
        {
            Search_Status = false;
            Console.WriteLine($"\nЗа {month} сканов не найдено ! Нет PDF !");  // out 
        }
    }

    private void CalcAllSums()
    {
        ProtocolsSums self_obj_sums = new();
        Sums = self_obj_sums.All_Protocols;
                
        for (int protocol_type_index = 0; protocol_type_index < self_obj_backup_item?.Files?.Count; protocol_type_index++) // null disable ??
        {
            if (self_obj_backup_item.Files[protocol_type_index] is not null)
            {
                Sums[ProtocolFullTypeLocation.others_sums[0]] += self_obj_backup_item.Files[protocol_type_index].Count;
                Sums[ProtocolFullTypeLocation.others_sums[protocol_type_index + 1]] = self_obj_backup_item.Files[protocol_type_index].Count;
            }
        }
    }
    
    private void SearchNoneProtocols()
    {
        if (month_value is not 1)
        {
            var previous_files = self_obj_backup_item?.CapturingFiles(FileTypesPatterns.file_patterns[FileTypesPatterns.protocol_file_type[1]], month_value - 1);

            AnalysWithUnknownProtocols other_monthes_analys = new(self_obj_backup_item.Files[1], previous_files);

            foreach (var item in other_monthes_analys.Simple_Protocols_Sums) // lambda ?
            {
                //Console.WriteLine($"{item.Key}: {item.Value}");
                Sums?.Add(item.Key, item.Value);
            }
        }
        else
        {
            ProtocolsAnalysis january_analys = new(self_obj_backup_item.Files[1]);

            foreach (var item in january_analys.Simple_Protocols_Sums) // lambda ?
            {
                //Console.WriteLine($"{item.Key}: {item.Value}");
                Sums?.Add(item.Key, item.Value);
            }
        }
    }
}            
