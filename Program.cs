using BackupBlock;
using DrivesControl;
using Logging;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TextData;
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
        
        SourceFiles self_obj_pdf_files = new(source_directory);

        if (AppConstants.month_names.Contains(current_period))
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
        
    break;
}
              

abstract class BackupProcess(SourceFiles self_obj_source_files)
{
    private protected MonthSums? self_obj_sums;
    public bool Search_Status { get; set; } 

    private protected static string CreatePeriodPattern(int month_value)
    {
        string month;

        if (month_value < 10)
        {
            month = $"0{month_value}";
        }
        else
        {
            month = month_value.ToString();
        }

        return string.Concat("\\d{2}\\.", month, "\\.", CurrentYear.Year, "\\.", AppConstants.scan_file_type, "$");
    }

    private protected List<FileInfo>? GetEIASFiles(string period_pattern)
    {
        return self_obj_source_files.GrabMatchedFiles(new(string.Concat(AppConstants.eias_number_pattern, period_pattern), RegexOptions.IgnoreCase));
    }

    private protected Dictionary<string, List<FileInfo>>? GetSimpleFiles(string period_pattern)
    {
        Dictionary<string, List<FileInfo>> files = [];

        for (int type_index = 0; type_index < AppConstants.types_full_names.Count; type_index++)
        {
            var current_files = self_obj_source_files.GrabMatchedFiles(new(string.Concat($"{AppConstants.simple_number_pattern}{AppConstants.types_short_names[type_index]}-", period_pattern), RegexOptions.IgnoreCase));

            if (current_files is not null)
            {
                files.Add(AppConstants.types_full_names[type_index], current_files);
            }
        }

        if (files.Count > 0)
        {
            return files;
        }
        else
        {
            return null;
        }
    }
}

    // destination >>
class BackupProcessMonth : BackupProcess
{
    public BackupProcessMonth(SourceFiles self_obj_source_files, string target_month) : base(self_obj_source_files)
    {
        int month_value = AppConstants.month_names.IndexOf(target_month) + 1;

        var eias_files = GetEIASFiles(CreatePeriodPattern(month_value));
        var simple_files = GetSimpleFiles(CreatePeriodPattern(month_value));
                                        
        if (month_value != 1)
        {
            self_obj_sums = new(eias_files, simple_files, GetSimpleFiles(CreatePeriodPattern(month_value - 1)));                
        }
        else
        {
            self_obj_sums = new(eias_files, simple_files);                
        }               

        if (self_obj_sums.All_Protocols[AppConstants.others_sums[0]] != 0)
        {
            Search_Status = true;

            // проверить файлы на ноль и копировать и лог соответственно

            if (self_obj_sums.All_Protocols[AppConstants.others_sums[1]] != 0)
            {
                EIASLog self_obj_eias_logger = new(month_value, eias_files!, self_obj_sums);

            }

            if (self_obj_sums.All_Protocols[AppConstants.others_sums[2]] != 0)
            {
                SimpleLog self_obj_simples_logger = new(month_value, simple_files!, self_obj_sums);
            }










            
            // >>>>>>>>>>> out >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            /*foreach (var item in self_obj_sums.All_Protocols)
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
            //Console.WriteLine("\n");
            if (self_obj_sums.All_Protocols[AppConstants.others_sums[1]] > 0)
            {
                Console.WriteLine($"\n{AppConstants.others_sums[1]} >>>>>>>>>>>>>");
                foreach (var file in eias_files!)
                {
                    Console.WriteLine(file.Name);
                }
            }





            // в другом классе ?? in class logger !!
            if (self_obj_sums.All_Protocols[AppConstants.others_sums[2]] > 0)
            {
                Console.WriteLine($"\n{AppConstants.others_sums[2]} >>>>>>>>>>>>>");
                
                
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

            if (self_obj_sums.Unknown_Protocols is not null)
            {
                Console.WriteLine("\nНеизвестные >>>>>>>>>>>>>");
                foreach (var item in self_obj_sums.Unknown_Protocols)
                {
                    Console.WriteLine(item);
                }
            }*/
            // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        }
        else
        {
            Search_Status = false;
        }
    }
}

// test year results !!!
class BackupProcessYear : BackupProcess, IGeneralSums, ISimpleProtocolsSums
{
    private readonly List<(int, List<FileInfo>?, Dictionary<string, List<FileInfo>>?, MonthSums)> year_full_backup = [];
    public Dictionary<string, int> All_Protocols { get; } = IGeneralSums.CreateTable();  // --//--
    public Dictionary<string, int> Simple_Protocols { get; } = ISimpleProtocolsSums.CreateTable(); // рассчитать в классе логера

    public BackupProcessYear(SourceFiles self_obj_source_files) : base(self_obj_source_files)
    {
        if (FindAllYearFiles())
        {
            Search_Status = true;

            // copy and logging









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

    private bool FindAllYearFiles()
    {
        List<Dictionary<string, List<FileInfo>>?> simple_files_trace = [];

        for (int month_index = 0; month_index < AppConstants.month_names.Count; month_index++)
        {
            var eias_files = GetEIASFiles(CreatePeriodPattern(month_index + 1));
            var simple_files = GetSimpleFiles(CreatePeriodPattern(month_index + 1));

            simple_files_trace.Add(simple_files);

            if (month_index > 0)
            {
                self_obj_sums = new(eias_files, simple_files, simple_files_trace[month_index - 1]);
            }
            else
            {
                self_obj_sums = new(eias_files, simple_files);
            }

            if (self_obj_sums.All_Protocols[AppConstants.others_sums[0]] != 0)
            {
                year_full_backup.Add((month_index, eias_files, simple_files, self_obj_sums)); // month index or string ??
            }

                                    
            








            if (self_obj_sums.All_Protocols[AppConstants.others_sums[0]] > 0)
            {
                // out console
                var current_month = AppConstants.month_names[month_index]; // only out
                Console.WriteLine($"\nУспех ! Найдено за {current_month} - {self_obj_sums.All_Protocols[AppConstants.others_sums[0]]} файлов !");
                // test out >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
                foreach (var item in self_obj_sums.All_Protocols)
                {
                    All_Protocols[item.Key] += item.Value;
                    //Console.WriteLine($"{item.Key}: {item.Value}");
                }

                if (self_obj_sums.All_Protocols[AppConstants.others_sums[2]] > 0)
                {
                    foreach (var item in self_obj_sums.Simple_Protocols_Sums!)
                    {
                        Simple_Protocols[item.Key] += item.Value;
                        //Console.WriteLine($"{item.Key}: {item.Value}");
                    }
                }
                // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            }
        }

        if (year_full_backup.Count != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}



