using System.Text.RegularExpressions;
using BackupBlock;
using DrivesControl;
using Logging;
using Tracing;


const string line = "- - - - - - - - - - - - - - -";
Console.WriteLine($"{line}\n* Backup PDF v.1.0 * / Test 2025\n{line}\n");

XMLConfig self_obj_drives_config = new();

string source_dir = self_obj_drives_config.Drives[0].Directory;
//string destination_dir = drives_config.Drives[1].Directory;

Console.WriteLine("\nВыберите, что нужно копировать:\n[1] - Сканы протоколов");
string backup_items_type = Console.ReadLine();   // отдельный класс для проверки пустой строки




switch (backup_items_type) // class ?????
{ 
    case "1":

        //for (int test_index = 0; test_index < 6;  test_index++)
        //{
            PdfFiles self_obj_pdf_files = new(FileTypesPatterns.file_types["PDF"], $"C:\\Users\\Mahabhara\\Desktop\\сканы\\test_unknowns_across_type_with_missing");
            
                // enter point !!!
            if (self_obj_pdf_files.Files is not null)
            {
                Console.WriteLine("\nPDF IS OK !!!");
                Console.WriteLine("\nВведите месяц, за который выполнить копирование:");
                // должна быть проверка правильности ввода месяца
                string current_period = Console.ReadLine();
                int current_month_value = MonthValues.Table[current_period];





                // enter point !!!!!!
                BackupFiles self_obj_backup = new(current_month_value, self_obj_pdf_files);

                if (self_obj_backup.Backup_Block is not null)
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
                            if (self_obj_analysis_with_unknowns.Missing_Protocols is not null)
                            {
                                Console.WriteLine("\nПропущены >>>");
                                foreach (var miss_file in self_obj_analysis_with_unknowns.Missing_Protocols)
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
                            if (self_obj_analysis.Missing_Protocols is not null)
                            {
                                Console.WriteLine("\nПропущены >>>");
                                foreach (var miss_file in self_obj_analysis.Missing_Protocols)
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
                }

            }
            else
            {
                Console.WriteLine("\nPDF NOT FOUND !!"); // return false    
            }



        //}

        


        break;
        

}


class BackupProcess
{







}

