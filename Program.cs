using System.Globalization;


AnyInfo.ShowProgramInfo();
Console.WriteLine('\n');

List<DirectoryInfo> work_drives = [];

foreach (string drive_type in XmlTags.DRIVE_TAGS)
{
    DrivesControl drives_control = new(drive_type);

    DirectoriesInfo.ShowDirectorySetupTrue(drive_type, drives_control.Work_Directory_in!.FullName);
    Console.WriteLine('\n');

    work_drives.Add(drives_control.Work_Directory_in);
}

AnyInfo.ShowEnterPeriod();
var period_value = InputNoNullText.GetRealText();

if (!int.TryParse(period_value, out int _))
{
    //Console.WriteLine("No number input !");        // input error 
    Environment.Exit(0);
}

int month_index = Convert.ToInt32(period_value, CultureInfo.CurrentCulture) - 1;
                                
if (month_index >= PeriodsNames.JANUARY_INDEX && month_index <= PeriodsNames.DECEMBER_INDEX)
{
    BackupProcessMonth _ = new(work_drives, PeriodsNames.MONTHES[month_index]); 
}
else if (period_value == CurrentDate.Year.ToString(CultureInfo.CurrentCulture))
{
    BackupProcessYear _ = new(work_drives);
}
else
{
    Environment.Exit(0);
}
           


// логи ошибок по дате и времени
// исключения !!!

