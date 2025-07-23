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
              
    
abstract class BackupProcessing<T>
{
    public bool Search_Status { get; set; }
    public BackupFilesMonth? Self_Obj_Backup_Item { get; set; }
    public T? PeriodLog { get; set; }

    private protected abstract void CalcAllSums();
    private protected abstract void SearchNoneProtocols();
    // сложение словарей ??
}


class MonthBackupProcessing : BackupProcessing<MonthSums>
{
    private readonly int month_value;
        
    public MonthBackupProcessing(PdfFiles source_files, string month)
    {
        month_value = MonthValues.Table[month];
        Self_Obj_Backup_Item = new(source_files);
        Self_Obj_Backup_Item.GetMonthBlock(month_value);

        if (Self_Obj_Backup_Item.Files is not null)
        {
            Search_Status = true;
            PeriodLog = new() { Period = month };

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
        for (int protocol_type_index = 0; protocol_type_index < Self_Obj_Backup_Item?.Files?.Count; protocol_type_index++)
        {
            if (Self_Obj_Backup_Item.Files[protocol_type_index] is not null)
            {
                PeriodLog.All_Protocols[ProtocolFullTypeLocation.others_sums[0]] += Self_Obj_Backup_Item.Files[protocol_type_index].Count;
                PeriodLog.All_Protocols[ProtocolFullTypeLocation.others_sums[protocol_type_index + 1]] = Self_Obj_Backup_Item.Files[protocol_type_index].Count;

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
            PeriodLog.Simple_Protocols = analys_obj.Simple_Protocols_Sums;
            PeriodLog.Missed_Protocols = analys_obj.Missed_Protocols;
        }

        if (month_value is not 1)
        {
            var previous_files = Self_Obj_Backup_Item.CapturingFiles(FileTypesPatterns.file_patterns[FileTypesPatterns.protocol_file_type[1]], month_value - 1);

            AnalysWithUnknownProtocols self_obj_other_monthes_analys = new(Self_Obj_Backup_Item.Files[1], previous_files);

            ConnectLogs(self_obj_other_monthes_analys);
            PeriodLog.Unknown_Protocols = self_obj_other_monthes_analys.Unknown_Protocols;
        }
        else
        {
            ProtocolsAnalysis self_obj_january_analys = new(Self_Obj_Backup_Item.Files[1]);

            ConnectLogs(self_obj_january_analys);      
        }
    }
#nullable restore
}


class YearBackupProcessing : BackupProcessing<YearSums>
{
    public List<(MonthSums, List<List<FileInfo>?>)> Year_Backup { get; set; }

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
    // фиктивная проверка
    private bool 



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






}