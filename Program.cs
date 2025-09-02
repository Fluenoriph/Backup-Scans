using InfoOut;
using InputValidate;
using System.Globalization;


GeneralInfo.ShowProgramInfo();

List<DriveControl> work_drives = [];

foreach (string drive_type in XmlTags.DRIVE_TAGS)
{
    DriveControl drive = new(drive_type);

    WorkDirectoriesInfo.ShowDirectorySetupTrue(drive_type, drive.Work_Directory_in!.FullName);
    Console.WriteLine('\n');

    work_drives.Add(drive);
}

bool program_menu_restart = false;

do
{
    GeneralInfo.ShowLine();
    Console.WriteLine('\n');

    GeneralInfo.ShowProgramMenu();
    var parameter = InputNoNullText.GetRealText();

    if (int.TryParse(parameter, out int _))
    {
        int month_index = Convert.ToInt32(parameter, CultureInfo.CurrentCulture) - 1;

        if (month_index >= PeriodsNames.JANUARY_INDEX && month_index <= PeriodsNames.DECEMBER_INDEX)
        {
            BackupProcessMonth _ = new(work_drives, PeriodsNames.MONTHES[month_index]);

            program_menu_restart = GeneralInfo.RestartOrExitProgram();
        }
        else if (parameter == CurrentDate.Year.ToString(CultureInfo.CurrentCulture))
        {
            BackupProcessYear _ = new(work_drives);

            program_menu_restart = GeneralInfo.RestartOrExitProgram();
        }
        else
        {
            _ = new ProgramShutDown(ErrorCodes.INPUT_VALUE_ERROR);
        }
    }
    else
    {
        if (parameter is Symbols.CHANGE_DIRECTORY_FUNCTION)
        {
            Console.WriteLine('\n');

            WorkDirectoriesInfo.ShowEnterDirectoryType();
            var drive_index = DriveIndex.Index_in;     



            

            program_menu_restart = true;
        }
        else
        {
            _ = new ProgramShutDown(ErrorCodes.INPUT_VALUE_ERROR);
        }
    }

} while (program_menu_restart == true);







// exe dir - C:\Users\Mahabhara\source\repos\Fluenoriph\Backup-Scans\bin\Debug\net9.0