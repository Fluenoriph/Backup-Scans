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
            FilesOfType pdf_files = new("*.pdf", source_dir);

            var result_pdf_files = pdf_files.Received_Files;

            if (result_pdf_files != null)
            {
                Console.WriteLine("\nВведите месяц, за который выполнить копирование:");
                string current_period = Console.ReadLine();
                // должна быть проверка правильности ввода месяца
                int current_month_value = MonthValues.Table[current_period];

                // получаем еиас сканы
                ProtocolScanGrabbing eias_capture = new(FileTypesPatterns.protocol_types[0], current_month_value, result_pdf_files);
                List<FileInfo>? eias_files = eias_capture.Files;
                // получаем простые сканы
                ProtocolScanGrabbing simple_capture = new(FileTypesPatterns.protocol_types[1], current_month_value, result_pdf_files);
                List<FileInfo>? simple_files = simple_capture.Files;

                List<FileInfo> result_files = [];

                if (eias_files != null && simple_files != null)
                {
                    IEnumerable<FileInfo> result = eias_files.Concat(simple_files);
                    // соединяем результат
                    result_files = [.. result];
                    // записать в словарь !!!
                    Console.WriteLine($"\nЗа {current_period} найдено {result_files.Count} файлов. Можно отправлять !\n");
                    foreach (var file in result_files)
                    {
                        Console.WriteLine(file.Name);
                    }

                    ProtocolTypeNumbers current_type_numbers = new(simple_files);

                    ProtocolsAnalysis analysis = new(current_type_numbers.Numbers);

                    foreach (var sums in analysis.Protocols_Sums)
                    {
                        Console.WriteLine($"{sums.Key} - {sums.Value}");
                    }

                    MissingProtocols missing = new(current_type_numbers.Numbers);

                    var missing_protocols = missing.Missing_Protocols;

                    if (missing_protocols != null)
                    {
                        Console.WriteLine($"\nПропущенных - {missing_protocols.Count}");
                    }
                    else
                    {
                        Console.WriteLine("\nПропущенных нет !");
                    }
                    
                    if (current_month_value > 1)
                    {
                        ProtocolScanGrabbing previous = new(FileTypesPatterns.protocol_types[1], current_month_value - 1, result_pdf_files);
                        
                        var previous_files = previous.Files;    
                        if (previous_files != null)
                        {
                            ProtocolTypeNumbers previous_type_numbers = new(previous_files);
                            MaximumNumbers max_numb = new(previous_type_numbers.Numbers);

                            UnknownProtocols unknown = new(max_numb.Numbers, missing.Min_Numbers);

                            var unknown_protocols = unknown.Unknown_Protocols;

                            if (unknown_protocols != null)
                            {
                                Console.WriteLine($"\nНеизвестных - {unknown_protocols.Count}");
                            }
                            else
                            {
                                Console.WriteLine("\nНеизвестных нет !");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nНеизвестных не найдено !");
                        }


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











