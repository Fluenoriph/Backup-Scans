using BackupBlock;
using DrivesControl;
using Logging;
using Tracing;


BackupProcess backuping = new();
backuping.PrepareToBackup();










class IO_Console
{
    public Dictionary<string, string> Info { get; } = new Dictionary<string, string>
    {
        ["App_start_info"] = "\nNebula Test 2025\n",
        ["Enter"] = "Введите значение:",
        ["Error"] = "Ошибка!",
        ["OK"] = "Успешно!"
        //[""]
    };

    public static void Out_info(string info) { Console.WriteLine(info); }

    public static string? Enter_value() { return Console.ReadLine(); }            
}











