using BackupBlock;
using DrivesControl;
using Logging;
using Tracing;


const string line = "- - - - - - - - - - - - - - -";

IO_Console.Out_info($"{line}\n* Nebulium 1.0 * / Test 2025\n{line}");

XMLConfig drives_config = new();

bool check_var;

do
{
    bool status = drives_config.GetSettings();
    check_var = status;
} while (check_var == false);

















//MaxNumbersPerMonth y = new(MonthValues.Table[CurrentMonth.Value]);
//y.Read();



/*MaxNumbersPerMonth x = new(CurrentMonth.value);
x.Read();
var z = x.GetNumbersTree("1");

foreach (var y in x.Values)
{
    Console.WriteLine(y);
}*/













struct CurrentMonth
{
    public static string Value { get; set; } = "";
}


class IO_Console
{ 
    public static void Out_info(string info) { Console.WriteLine(info); }
    public static string? Enter_value() { return Console.ReadLine(); }            
}











