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
    private protected BackupFilesMonth? self_obj_files;
    private protected MonthSums? self_obj_backup; 
    public bool Search_Status { get; set; } // нужно ли ?
}

    // destination >>
class BackupProcessMonth : BackupProcess
{
    public BackupProcessMonth(PdfFiles self_obj_source_files, string target_month)
    {
        int month_value = MonthValues.Table[target_month];
        self_obj_files = new(self_obj_source_files);       
        var files_block = self_obj_files.GetFilesBlock(month_value);

        if (files_block is not null)
        {
            Search_Status = true;

            Console.WriteLine($"\nЗа {target_month} найдено: ЕИАС-{files_block.EIAS?.Count}/ Простых: {files_block.Simple?.Count}");

            if (month_value != 1)
            {
                self_obj_backup = new MonthSumsWithUnknowns(files_block, self_obj_files.CapturingFiles(FileTypesPatterns.file_patterns[FileTypesPatterns.protocol_file_type[1]], month_value - 1));                
            }
            else
            {
                self_obj_backup = new MonthSumsExceptUnknowns(files_block);                
            }

            foreach (var item in self_obj_backup.All_Protocols)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }

            foreach (var item in self_obj_backup.Self_Obj_Analys_Simple_Type.Simple_Protocols_Sums)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
        }
        else
        {
            Search_Status = false;  
        }        
    }
}


class BackupProcessYear : BackupProcess
{
    private readonly List<(int, MonthFilesBlock)> year_sums = [];
    // словарь за весь год !!!

    public BackupProcessYear(PdfFiles self_obj_source_files)
    {
        self_obj_files = new(self_obj_source_files);

        for (int month_index = 0; month_index < MonthValues.Month_Count; month_index++)
        {
            var files_block = self_obj_files.GetFilesBlock(month_index + 1);
            
            if (files_block is not null)
            {
                year_sums.Add((month_index + 1, files_block));
            }
        }

        // другая операция и метод !!
        //Console.WriteLine(year_sums.Count);
        if (year_sums.Count != 1) // найдено за год хотя бы 1 месяц
        {
            for (int files_block_index = 0; files_block_index < year_sums.Count; files_block_index++)      // variable - count
            {
                var current_block = year_sums[files_block_index];
                var current_month = MonthValues.Month_Names[current_block.Item1 - 1];

                Console.WriteLine($"\nНайдено за {current_month} !");
                                                                        // simple files gets !!!
                if ((files_block_index > 0) && (current_block.Item1 - year_sums[files_block_index - 1].Item1 == 1))   // bool string var !!
                {
                    self_obj_backup = new MonthSumsWithUnknowns(current_block.Item2, year_sums[files_block_index - 1].Item2.Simple);

                }
                else
                {
                    self_obj_backup = new MonthSumsExceptUnknowns(current_block.Item2);
                }
                // test out >>>>
                foreach (var item in self_obj_backup.All_Protocols)
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }

                foreach (var item in self_obj_backup.Self_Obj_Analys_Simple_Type.Simple_Protocols_Sums)
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }

            }
        }        // без проверки на пустой год !!
        else
        {
            Console.WriteLine($"\nНайдено за {MonthValues.Month_Names[year_sums[0].Item1 - 1]} !");

            self_obj_backup = new MonthSumsExceptUnknowns(year_sums[0].Item2);
            // test out !!
            foreach (var item in self_obj_backup.All_Protocols)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }

            foreach (var item in self_obj_backup.Self_Obj_Analys_Simple_Type.Simple_Protocols_Sums)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
        }

        
    }

    
}