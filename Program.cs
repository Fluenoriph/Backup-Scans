using System.Text.RegularExpressions;
using BackupBlock;
using DrivesControl;
using Logging;
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



        

        
        if (self_obj_pdf_files.Files is not null)
        {
            if (MonthValues.Month_Names.Contains(current_period))
            {
                MonthBackupProcessing self_obj_month_backup = new(self_obj_pdf_files, current_period);

                if (self_obj_month_backup.Search_Status)
                {
                    Console.WriteLine($"\nЗа {current_period} найдено {self_obj_month_backup.PeriodLog.All_Protocols[ProtocolFullTypeLocation.others_sums[0]]} сканов !");

                    Console.WriteLine($"\n{self_obj_month_backup.PeriodLog.Period}");
                    foreach (var item in self_obj_month_backup.PeriodLog.All_Protocols)
                    {
                        Console.WriteLine($"{item.Key}: {item.Value}");
                    }

                    foreach (var item in self_obj_month_backup.PeriodLog.Simple_Protocols)
                    {
                        Console.WriteLine($"{item.Key}: {item.Value}");
                    }

                    foreach (var item in self_obj_month_backup.PeriodLog.Missed_Protocols)
                    {
                        Console.WriteLine(item);
                    }

                    foreach (var item in self_obj_month_backup.PeriodLog.Unknown_Protocols)
                    {
                        Console.WriteLine(item);
                    }

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
class BackupProcess(string source_directory, string target_period)
{
    private readonly PdfFiles self_obj_pdf_files = new(source_directory);
    private Action<string> MonthScansNotFound = (period) => Console.WriteLine($"\nЗа {period} сканов не найдено !");


}