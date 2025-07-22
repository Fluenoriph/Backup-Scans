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
        string current_period = Console.ReadLine();
        Action<string> MonthScansNotFound = (month) => Console.WriteLine($"\nЗа {month} сканов не найдено !");

        PdfFiles self_obj_pdf_files = new(source_directory);

        if (self_obj_pdf_files.Files is not null)
        {
            if (MonthValues.Month_Names.Contains(current_period))
            {
                MonthBackupProcessing self_obj_month_backup = new(self_obj_pdf_files, current_period);

                if (self_obj_month_backup.Search_Status)
                {
                    Console.WriteLine($"\nЗа {current_period} найдено {self_obj_month_backup.MonthLog.All_Protocols[ProtocolFullTypeLocation.others_sums[0]]} сканов !");

                    Console.WriteLine($"\n{self_obj_month_backup.MonthLog.Period}");
                    foreach (var item in self_obj_month_backup.MonthLog.All_Protocols)
                    {
                        Console.WriteLine($"{item.Key}: {item.Value}");
                    }

                    foreach (var item in self_obj_month_backup.MonthLog.Simple_Protocols)
                    {
                        Console.WriteLine($"{item.Key}: {item.Value}");
                    }

                    foreach (var item in self_obj_month_backup.MonthLog.Missed_Protocols)
                    {
                        Console.WriteLine(item);
                    }

                    foreach (var item in self_obj_month_backup.MonthLog.Unknown_Protocols)
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
              
    
abstract class BackupProcessing
{
    public bool Search_Status { get; set; }
    public MonthSums MonthLog { get; } = new();

    private protected abstract void CalcAllSums();
    private protected abstract void SearchNoneProtocols();
    // сложение словарей ??
}


class MonthBackupProcessing : BackupProcessing
{
    private readonly int month_value;
    private readonly BackupFilesMonth self_obj_backup_item; // in abs
    
    public MonthBackupProcessing(PdfFiles source_files, string month)
    {
        month_value = MonthValues.Table[month];
        self_obj_backup_item = new(source_files);
        self_obj_backup_item.GetMonthBlock(month_value);

        if (self_obj_backup_item.Files is not null)
        {
            Search_Status = true;

            MonthLog.Period = month;
            CalcAllSums();                              

                // метод или класс компоновщик данных отчета
                // но сначала копирование !
        }
        else
        {
            Search_Status = false;
        }
    }

#nullable disable
    private protected override void CalcAllSums()
    {
        for (int protocol_type_index = 0; protocol_type_index < self_obj_backup_item?.Files?.Count; protocol_type_index++)
        {
            if (self_obj_backup_item.Files[protocol_type_index] is not null)
            {
                MonthLog.All_Protocols[ProtocolFullTypeLocation.others_sums[0]] += self_obj_backup_item.Files[protocol_type_index].Count;
                MonthLog.All_Protocols[ProtocolFullTypeLocation.others_sums[protocol_type_index + 1]] = self_obj_backup_item.Files[protocol_type_index].Count;

                if (protocol_type_index is 1)
                {
                    SearchNoneProtocols();
                }
            }
        }
    }

    private protected override void SearchNoneProtocols()
    {
        void ConnectLogs(ProtocolsAnalysis analys_obj)
        {
            MonthLog.Simple_Protocols = analys_obj.Simple_Protocols_Sums;
            MonthLog.Missed_Protocols = analys_obj.Missed_Protocols;
        }

        if (month_value is not 1)
        {
            var previous_files = self_obj_backup_item.CapturingFiles(FileTypesPatterns.file_patterns[FileTypesPatterns.protocol_file_type[1]], month_value - 1);

            AnalysWithUnknownProtocols self_obj_other_monthes_analys = new(self_obj_backup_item.Files[1], previous_files);

            ConnectLogs(self_obj_other_monthes_analys);
            MonthLog.Unknown_Protocols = self_obj_other_monthes_analys.Unknown_Protocols;
        }
        else
        {
            ProtocolsAnalysis self_obj_january_analys = new(self_obj_backup_item.Files[1]);

            ConnectLogs(self_obj_january_analys);      
        }
    }
#nullable restore
}


/*class YearBackupProcessing : BackupProcessing
{
    private readonly BackupFilesYear files_block;

    public YearBackupProcessing(PdfFiles source_files)
    {
        files_block = new(source_files);

        if (files_block.Files is not null)
        {
            Search_Status = true;




        }
        else
        {
            Search_Status = false;
        }
    }

    private protected override void CalcAllSums()
    {
        static bool CheckNoneYearBlock(List<List<FileInfo>?> year_file_block)
        {
            int month_count = 0;

            foreach (var files in year_file_block)
            {
                if (files is null)
                {
                    month_count++;
                }
            }

            if (month_count != MonthValues.Month_Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }






    }






}*/