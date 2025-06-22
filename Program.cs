using System.Text.RegularExpressions;
using BackupBlock;
using DrivesControl;
using Logging;
using Tracing;


const string line = "- - - - - - - - - - - - - - -";
IO_Console.Out_info($"{line}\n* Nebulium 1.0 * / Test 2025\n{line}");

XMLConfig drives_config = new();
const string simple_file_pattern = "^\\d{1,4}-(ф|фа|р|ра|м|ма)-";

Console.WriteLine("\nВведите месяц:");
string current_period = Console.ReadLine();

int previous_month_value = MonthValues.Table[current_period] - 1;


RgxPattern simple_file_rgx = new(current_period, simple_file_pattern);

if (drives_config.Drives_Ready)
{
    Console.WriteLine("\nAll Ready !!");

    string source_dir = drives_config.Drives[0].Directory;

    BackupItem backup_files = new(simple_file_rgx, source_dir);

    if (backup_files.Search_Status == FileBlockStatus.FILES_DO_NOT_EXIST)
    {
        Console.WriteLine($"\nФайлов типа {simple_file_rgx.File_Type} не найдено !");
    }
    else if (backup_files.Search_Status == FileBlockStatus.NONE_FILES_IN_CURRENT_PERIOD)
    {
        Console.WriteLine($"\nЗа {current_period} протоколов не найдено !");
    }
    else
    {
        Console.WriteLine($"\nЗа {current_period} найдено {backup_files.Result_Files.Count} файлов !");

        Protocols protocols = new(backup_files.Result_Files);

        foreach (var numbers in protocols.Protocol_type_numbers)
        {
            foreach (var number in numbers)
            {  
                Console.WriteLine(number); 
            }
        }

    }


        




}
else
{
    Console.WriteLine("\nNot Ready. Exit >>>");
}


















// C:\Users\Asus machine\Desktop\Files\сканы
// C:\Users\Asus machine\Desktop\Files\result_test


class IO_Console  // нужно ли ??
{ 
    public static void Out_info(string info) { Console.WriteLine(info); }
    public static string? Enter_value() { return Console.ReadLine(); }            
}











