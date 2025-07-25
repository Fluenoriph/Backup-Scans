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
              
    // destination >>
class BackupProcessMonth
{
    public bool Search_Status { get; set; } // abs ?
    
    public BackupProcessMonth(PdfFiles self_obj_source_files, string target_month)
    {
        BackupFilesMonth self_obj_files = new(self_obj_source_files);

        int month_value = MonthValues.Table[target_month];
        var files = self_obj_files.GetBlock(month_value);

        if (files is not null)
        {
            Search_Status = true;

            Console.WriteLine($"\nЗа {target_month} найдено: ЕИАС-{files[0]?.Count}/ Простых: {files[1]?.Count}");

            if (month_value != 1)
            {
                MonthSumsWithUnknowns self_obj_backup = new(files, self_obj_files.CapturingFiles(FileTypesPatterns.file_patterns[FileTypesPatterns.protocol_file_type[1]], month_value - 1));

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
                MonthSumsExceptUnknowns self_obj_backup = new(files);

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
        else
        {
            Search_Status = false;  
        }
    }
}