using System.Text.RegularExpressions;
using BackupBlock;
using DrivesControl;
using Logging;
using Tracing;


const string line = "- - - - - - - - - - - - - - -";
Console.WriteLine($"{line}\n* Nebulium 1.0 * / Test 2025\n{line}\n");

XMLConfig drives_config = new();

string source_dir = drives_config.Drives[0].Directory;
string destination_dir = drives_config.Drives[1].Directory;

Console.WriteLine("\nВыберите, что нужно копировать:\n[1] - Сканы протоколов");
string backup_items_type = Console.ReadLine();   // отдельный класс для проверки пустой строки

switch (backup_items_type)
{ 
    // может быть это отдельный метод ??
    case "1":
        BackupFilesType pdf_files = new(FileTypesPatterns.File_Types["PDF"], source_dir);
        var result_pdf_files = pdf_files.Found_Status;

        if (result_pdf_files is not null)
        {
            Console.WriteLine("\nPDF IS OK !!!");
            Console.WriteLine("\nВведите месяц, за который выполнить копирование:");
            // должна быть проверка правильности ввода месяца
            string current_period = Console.ReadLine();
            int current_month_value = MonthValues.Table[current_period];

            ProtocolScanPattern protocol_pattern = new(current_month_value);
            BackupItem protocol_files = new(result_pdf_files);
            // получаем еиас сканы






        }
        else
        {
            Console.WriteLine("\nPDF NOT FOUND !!");
        }


            break;
        /*if (result_pdf_files != null)
        {
            
            
            


            // начало класса ////
            
            ProtocolScanGrabbing eias_capture = new(FileTypesPatterns.protocol_file_type[0], current_month_value, result_pdf_files);
            List<FileInfo>? eias_files = eias_capture.Files;
            // получаем простые сканы
            ProtocolScanGrabbing simple_capture = new(FileTypesPatterns.protocol_file_type[1], current_month_value, result_pdf_files);
            List<FileInfo>? simple_files = simple_capture.Files;

            List<FileInfo> result_files = [];




            // null result !!!
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
                var type_numb = current_type_numbers.Numbers;

                ProtocolsAnalysis analysis = new(type_numb);



                foreach (var sums in analysis.Protocols_Sums)
                {
                    Console.WriteLine($"{sums.Key} - {sums.Value}");
                }



                MissingProtocols missing = new(type_numb);

                var missing_protocols = missing.Missing_Protocols;

                if (missing_protocols != null)
                {
                    Console.WriteLine($"\nПропущенных - {missing_protocols.Count}");
                }
                else
                {
                    Console.WriteLine("\nПропущенных нет !");
                }






                // подкласс ??
                if (current_month_value > 1)
                {
                    ProtocolScanGrabbing previous = new(FileTypesPatterns.protocol_file_type[1], current_month_value - 1, result_pdf_files);

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
        }*/

}

    























// C:\Users\Asus machine\Desktop\Files\сканы
// C:\Users\Asus machine\Desktop\Files\result_test


