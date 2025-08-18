using DrivesControl;
using TextData;
using Tracing;


AppInfoConsoleOut.ShowProgramInfo();

DrivesConfiguration self_obj_drives_config = new();
AppInfoConsoleOut.ShowLine();

AppInfoConsoleOut.ShowEnterPeriod();

var period_value = InputNoNullText.GetRealText();
if (!int.TryParse(period_value, out int _))
{
    Console.WriteLine("No number input !");
    Environment.Exit(0);
}

int month_index = Convert.ToInt32(period_value) - 1;
                                
if (month_index >= AppConstants.january_index && month_index <= AppConstants.december_index)
{
    BackupProcessMonth _ = new(self_obj_drives_config.Drives, AppConstants.month_names[month_index]); 
}
else if (month_index == CurrentDate.Year)
{
    BackupProcessYear _ = new(self_obj_drives_config.Drives);
}
else
{
    Environment.Exit(0);
}
           

// резервная папка исходная по году ./2025.....
// логи также по году
// логи ошибок по дате и времени
// исключения !!!
// если копировать за месяц, то итеративно суммировать за год