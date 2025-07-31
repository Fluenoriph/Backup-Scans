using Aspose.Words.Bibliography;
using Aspose.Words.Drawing;
using BackupBlock;
using DrivesControl;
using Logging;
using System.Text.RegularExpressions;
using Tracing;

// class App
const string line = "- - - - - - - - - - - - - - -";
Console.WriteLine($"{line}\n* Backup PDF v.1.0 * / Test 2025\n{line}\n");

XMLConfig self_obj_drives_config = new();
string source_directory = self_obj_drives_config.Drives[0].Directory;  // объект в классах ???
//string destination_dir = drives_config.Drives[1].Directory;

Console.WriteLine("\nВыберите, что нужно копировать:\n[1] - Сканы протоколов");
string backup_items_type = "1";//Console.ReadLine();   // отдельный класс для проверки пустой строки

switch (backup_items_type)
{
    case "1":
        // while ??
        // method ????? in class
        Console.WriteLine("\nВведите месяц, за который выполнить копирование:");   // если год, это статическая структура даты
        // должна быть проверка правильности ввода месяца
        string? current_period = Console.ReadLine();
        Action<string> MonthScansNotFound = (period) => Console.WriteLine($"\nЗа {period} сканов не найдено !");
        PdfFiles self_obj_pdf_files = new(source_directory);

        if (self_obj_pdf_files.Files is not null)
        {
            if (MonthValues.Month_Names.Contains(current_period))
            {
                BackupProcessMonth self_obj_backup_month = new(self_obj_pdf_files, current_period);

                if (self_obj_backup_month.Search_Status)
                {
                    Console.WriteLine("\nМожно копировать !");
                }
                else
                {
                    MonthScansNotFound(current_period);
                }
            }
            else if (current_period is "год")
            {
                Console.WriteLine("\nЗапущено копирование за год !");

                BackupProcessYear self_obj_backup_year = new(self_obj_pdf_files);
            }
            else
            {
                Console.WriteLine("\nОшибка ввода периода !");
                System.Environment.Exit(0);
            }
        }
        else
        {
            MonthScansNotFound(current_period);
        }

    break;
}
              

abstract class BackupProcess
{
    private protected BackupFilesMonth? self_obj_backup_files;
    private protected MonthSumsExceptUnknowns? self_obj_sums; 
    public bool Search_Status { get; set; } // не надо, сразу копировать ??
}

    // destination >>
class BackupProcessMonth : BackupProcess
{
    public BackupProcessMonth(PdfFiles self_obj_source_files, string target_month)
    {
        int month_value = MonthValues.Table[target_month];
        self_obj_backup_files = new(self_obj_source_files);       
        var files_block = self_obj_backup_files.GetFilesBlock(month_value);

        if (files_block is not null)
        {
            Search_Status = true;
                        
            if (month_value != 1)
            {
                self_obj_sums = new MonthSumsWithUnknowns(files_block, self_obj_backup_files.CapturingFiles(FileTypesPatterns.file_patterns[FileTypesPatterns.protocol_file_type[1]], month_value - 1));                
            }
            else
            {
                self_obj_sums = new(files_block);                
            }

            // start log class !!
            // >>>>>>>>>>> out >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            foreach (var item in self_obj_sums.All_Protocols)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }

            if (self_obj_sums.Simple_Protocols_Sums is not null)
            {
                foreach (var item in self_obj_sums.Simple_Protocols_Sums)
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }
            }
            Console.WriteLine("\n");
            if (files_block.EIAS is not null)
            {
                foreach (var file in files_block.EIAS)
                {
                    Console.WriteLine(file.Name);
                }
            }

            if (files_block.Simple is not null)
            {
                foreach (var file in files_block.Simple)
                {
                    Console.WriteLine(file.Name);
                }
            }
            // *** missed & unknowns ***
            if (self_obj_sums.Missed_Protocols is not null)
            {
                Console.WriteLine("\nПропущенные >>>>>>>>>>>>>");
                foreach (var item in self_obj_sums.Missed_Protocols)
                {
                    Console.WriteLine(item);
                }
            }
                
            if (self_obj_sums is MonthSumsWithUnknowns)
            {
                MonthSumsWithUnknowns self_obj_unknown = (MonthSumsWithUnknowns)self_obj_sums;

                if (self_obj_unknown.Unknown_Protocols is not null)
                {
                    Console.WriteLine("\nНеизвестные >>>>>>>>>>>>>");
                    foreach (var item in self_obj_unknown.Unknown_Protocols)
                    {
                        Console.WriteLine(item);
                    }
                }
            }
            // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        }
        else
        {
            Search_Status = false;  
        }        
    }
}


class BackupProcessYear : BackupProcess, IGeneralSums, ISimpleProtocolsSums
{
    private readonly List<(int, MonthFilesBlock)> year_sums = [];
    public Dictionary<string, int>? All_Protocols { get; private set; }
    public Dictionary<string, int>? Simple_Protocols { get; private set; }

    public BackupProcessYear(PdfFiles self_obj_source_files)
    {
        self_obj_backup_files = new(self_obj_source_files);

        if (FindYearFiles())
        {
            Search_Status = true;

            ComputeBackupSums();

            // >>>>>>>>>>>>>>>>>>>>>>> out
            Console.WriteLine("\n>>> Сумма за год >>> >>> >>> >>> >>>\n");
            foreach (var item in All_Protocols!)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
            foreach (var item in Simple_Protocols!)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
            // >>>>>>>>>>>>>>>>>>>>>>>

        }
        else
        {
            Search_Status = false;
        } 
    }

    private bool FindYearFiles()
    {
        for (int month_index = 0; month_index < MonthValues.Month_Count; month_index++)
        {
            var files_block = self_obj_backup_files!.GetFilesBlock(month_index + 1);
            
            if (files_block is not null)
            {
                year_sums.Add((month_index + 1, files_block));
            }
        }

        if (year_sums.Count > 0) 
        { 
            return true; 
        }
        else
        {
            return false;
        }
    }

    private void ComputeBackupSums()
    {
        if (year_sums.Count != 1) 
        {
            All_Protocols = IGeneralSums.CreateTable();
            Simple_Protocols = ISimpleProtocolsSums.CreateTable();

            for (int files_block_index = 0; files_block_index < year_sums.Count; files_block_index++)      
            {
                var current_block = year_sums[files_block_index];
                // out >>>>>>>>>>>>>>>>>>>>>>>>>
                var current_month = MonthValues.Month_Names[current_block.Item1 - 1];
                Console.WriteLine($"\nУспех ! Найдено за {current_month} !");
                // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                
                if ((files_block_index > 0) && (current_block.Item1 - year_sums[files_block_index - 1].Item1 == 1))   
                {
                    self_obj_sums = new MonthSumsWithUnknowns(current_block.Item2, year_sums[files_block_index - 1].Item2.Simple);
                }
                else
                {
                    self_obj_sums = new(current_block.Item2);
                }


                // test out >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                foreach (var item in self_obj_sums.All_Protocols)
                {
                    All_Protocols[item.Key] += item.Value;
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }

                if (self_obj_sums.Simple_Protocols_Sums is not null)
                {
                    foreach (var item in self_obj_sums.Simple_Protocols_Sums)
                    {
                        Simple_Protocols[item.Key] += item.Value;
                        Console.WriteLine($"{item.Key}: {item.Value}");
                    }
                }
                // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            }
        }        
        else
        {
            Console.WriteLine($"\nНайдено за {MonthValues.Month_Names[year_sums[0].Item1 - 1]} !");

            self_obj_sums = new(year_sums[0].Item2);

            All_Protocols = self_obj_sums.All_Protocols;
            Simple_Protocols = self_obj_sums.Simple_Protocols_Sums;


            // test out !! >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            foreach (var item in self_obj_sums.All_Protocols)
            {
                //All_Protocols[item.Key] += item.Value;
                Console.WriteLine($"{item.Key}: {item.Value}");
            }

            if (self_obj_sums.Simple_Protocols_Sums is not null)
            {
                foreach (var item in self_obj_sums.Simple_Protocols_Sums)
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }
            }
            // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        }
    }
}
