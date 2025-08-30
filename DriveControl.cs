using System.Xml.Linq;


class DriveControl
{
    public DirectoryInfo? Work_Directory_in { get; set; }  // исключение доступа прав ??

    private string? Drive_Type_in { get; }
    private readonly DrivesConfigurationFile config_file_in = new(LogFiles.DRIVES_CONFIG_FILE);
    private XElement? X_Drive_in { get; set; }
    private bool Real_Directory_status { get; set; }

    public DriveControl(string drive_type)
    {
        Drive_Type_in = drive_type;

        X_Drive_in = config_file_in.Document_in!.Root?.Element(Drive_Type_in!);   // если нулл, то shut down
        var directory_lcl = X_Drive_in!.Value;
                
        do
        {
            Real_Directory_status = CheckDirectory(directory_lcl);

            if (Real_Directory_status)
            {
                Work_Directory_in = new(directory_lcl!);
            }
            else
            {
                WorkDirectoriesInfo.ShowDirectoryExistFalse(Drive_Type_in!);
                GeneralInfo.ShowLine();

                directory_lcl = InputNoNullText.GetRealText();

                WriteDirectory(directory_lcl);
                
                Console.WriteLine('\n');
            }

        } while (Real_Directory_status == false);
    }

    private void WriteDirectory(string directory)
    {
        X_Drive_in!.Value = directory;
        config_file_in.Document_in!.Save(LogFiles.DRIVES_CONFIG_FILE);
    }

    private static bool CheckDirectory(string? directory_full_name)
    {
        if (!string.IsNullOrEmpty(directory_full_name))
        {
            if (Directory.Exists(directory_full_name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public bool ChangeWorkDirectory(string new_directory)
    {
        Real_Directory_status = CheckDirectory(new_directory);

        if (Real_Directory_status)
        {
            WriteDirectory(new_directory);

            Work_Directory_in = new(new_directory);

            return true;
        }
        else
        {
            return false;
        }
    }
}
