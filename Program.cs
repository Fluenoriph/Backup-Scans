using BackupBlock;
using DrivesControl;
using Logging;
using Tracing;


const string line = "- - - - - - - - - - - - - - -";

IO_Console.Out_info($"{line}\n* Nebula 2.1 * / Test 2025\n{line}");
BackupProcess backuping = new();
backuping.PrepareToBackup();

//MaxNumbersPerMonth x = new(CurrentMonth.value);

//x.Read();
//var z = x.GetNumbersTree("1");


foreach (var y in x.Values)
{
    Console.WriteLine(y);
}













struct CurrentMonth
{
    public static readonly string value = "Февраль";
}


class IO_Console
{ 
    public static void Out_info(string info) { Console.WriteLine(info); }
    public static string? Enter_value() { return Console.ReadLine(); }            
}











