using System.Text.RegularExpressions;
using BackupBlock;
using DrivesControl;
using Logging;
using Tracing;


const string line = "- - - - - - - - - - - - - - -";
IO_Console.Out_info($"{line}\n* Nebulium 1.0 * / Test 2025\n{line}\n");

XMLConfig drives_config = new();




if (drives_config.Drives_Ready)
{
    Console.WriteLine("\nВсе готово к копированию !");
    
    string source_dir = drives_config.Drives[0].Directory;

    Console.WriteLine("\nВыберите, что нужно копировать:\n[1] - Сканы протоколов");
    string backup_items_type = Console.ReadLine();

    switch (backup_items_type)
    {
        case "1":
            FilesAtType pdf_files = new("*.pdf", source_dir);

            var result_pdf_files = pdf_files.Received_Files;

            if (result_pdf_files != null)
            {
                Console.WriteLine("\nВведите месяц, за который выполнить копирование:");
                string current_period = Console.ReadLine();
                // должна быть проверка правильности ввода месяца
                int current_month_value = MonthValues.Table[current_period];

                if (current_month_value == 1)  // > 1
                {
                    ProtocolsAnalysis block_analyzer = new(current_month_value, result_pdf_files);
                                        
                    List<FileInfo>? result_files = block_analyzer.Result_Backup_Block;

                    if (result_files != null)
                    {
                        Console.WriteLine($"\nЗа {current_period} найдено {result_files.Count} файлов. Можно отправлять !\n");

                        foreach (var file in result_files)
                        {
                            Console.WriteLine(file.Name);
                        }

                        Console.WriteLine("\n");

                        foreach (var sums in block_analyzer.Files_Sums)
                        {
                            Console.WriteLine($"{sums.Key} - {sums.Value}");
                        }

                        MissingProtocols missing = new(block_analyzer.Protocol_Type_Numbers);
                        var missing_protocols = missing.Missing_Protocols;

                        if (missing_protocols != null)
                        {
                            Console.WriteLine($"\nПропущенных - {missing_protocols.Count}");
                        }
                        else
                        {
                            Console.WriteLine("\nПропущенных нет !");
                        }
                        
                    }
                    else
                    {
                        Console.WriteLine($"\nЗа {current_period} ничего не найдено !");
                    }


                }

            }
            else
            {
                Console.WriteLine("\nНикаких файлов сканов не найдено !");
            }
            break;
    }

    





}
else
{
    Console.WriteLine("\nОшибка программы. Копирование невозможно ! Завершение работы >>");
}


















// C:\Users\Asus machine\Desktop\Files\сканы
// C:\Users\Asus machine\Desktop\Files\result_test


class IO_Console  // нужно ли ??
{ 
    public static void Out_info(string info) { Console.WriteLine(info); }
    public static string? Enter_value() { return Console.ReadLine(); }            
}











