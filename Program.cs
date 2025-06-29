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

            if (pdf_files.Received_Files != null)
            {
                Console.WriteLine("\nВведите месяц, за который выполнить копирование:");
                string current_period = Console.ReadLine();

                ProtocolsAnalysis block_analyzer = new(current_period, pdf_files.Received_Files);

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
                }
                else
                {
                    Console.WriteLine($"\nЗа {current_period} ничего не найдено !");
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











