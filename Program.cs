using System.Globalization;
using TextData;
using Tracing;


AppInfoConsoleOut.ShowProgramInfo();
Console.WriteLine('\n');

List<Drive> drives = [];

foreach (string drive_type in AppConstants.drive_tags)
{
    DrivesControl self_obj_drives_control = new(drive_type);

    AppInfoConsoleOut.ShowDirectorySetupTrue(drive_type, self_obj_drives_control.Drive.Directory_Name!);
    Console.WriteLine('\n');

    drives.Add(self_obj_drives_control.Drive);
}

AppInfoConsoleOut.ShowEnterPeriod();
var period_value = InputNoNullText.GetRealText();

if (!int.TryParse(period_value, out int _))
{
    //Console.WriteLine("No number input !");        // error 
    Environment.Exit(0);
}

int month_index = Convert.ToInt32(period_value, CultureInfo.CurrentCulture) - 1;
                                
if (month_index >= AppConstants.january_index && month_index <= AppConstants.december_index)
{
    BackupProcessMonth _ = new(drives, AppConstants.month_names[month_index]); 
}
else if (period_value == CurrentDate.Year.ToString(CultureInfo.CurrentCulture))
{
    BackupProcessYear _ = new(drives);
}
else
{
    Environment.Exit(0);
}
           


// логи ошибок по дате и времени
// исключения !!!
// LogsData разделить
