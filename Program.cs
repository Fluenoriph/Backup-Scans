using BackupBlock;
using DrivesControl;
using Logging;
using Tracing;


const string line = "- - - - - - - - - - - - - - -";

IO_Console.Out_info($"{line}\n* Nebulium 1.0 * / Test 2025\n{line}");

XMLConfig drives_config = new();

if (drives_config.PrepareToBackup())
{
    Console.WriteLine("\nAll Ready !!");

    CurrentMonth.Value = "Январь";


}
else
{
    Console.WriteLine("\nNot Ready. Exit >>>");
}



//C:\Users\Asus machine\Desktop\Files\сканы
//C:\Users\Asus machine\Desktop\Files\result_test













//MaxNumbersPerMonth y = new(MonthValues.Table[CurrentMonth.Value]);
//y.Read();



/*MaxNumbersPerMonth x = new(CurrentMonth.value);
x.Read();
var z = x.GetNumbersTree("1");

foreach (var y in x.Values)
{
    Console.WriteLine(y);
}*/




class IO_Console
{ 
    public static void Out_info(string info) { Console.WriteLine(info); }
    public static string? Enter_value() { return Console.ReadLine(); }            
}











