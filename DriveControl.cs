using System.Xml.Linq;
using InputValidate;
using InfoOut;
using System.Security;


class DriveControl
{
    public DirectoryInfo? Work_Directory_in { get; set; }  

    private string? Drive_Type_in { get; }
    private readonly DrivesConfigurationFile config_file_in = new(LogFiles.DRIVES_CONFIG_FILE);
    private XElement? X_Drive_in { get; }
    private bool Real_Directory_status { get; set; }

    public DriveControl(string drive_type)
    {
        Drive_Type_in = drive_type;
        
        X_Drive_in = config_file_in.Document_in!.Element(XmlTags.DRIVES_CONFIG_TAG)?.Element(Drive_Type_in!);
                
        if (X_Drive_in is null)
        {
            _ = new ProgramShutDown(ErrorCodes.XML_ELEMENT_ACCESS_ERROR);
        }

        var directory_lcl = X_Drive_in!.Value;
                
        do
        {
            Real_Directory_status = CheckDirectory(directory_lcl);

            if (Real_Directory_status)
            {
                CreateAndCheckAccessDriveDirectory(directory_lcl);
            }
            else
            {
                WorkDirectoriesInfo.ShowDirectoryExistFalse(Drive_Type_in!, directory_lcl);
                GeneralInfo.ShowLine();

                directory_lcl = InputNoNullText.GetRealText();

                WriteDirectory(directory_lcl);
                
                Console.WriteLine('\n');
            }

        } while (Real_Directory_status == false);
    }

    private void CreateAndCheckAccessDriveDirectory(string directory)
    {
        try
        {
            Work_Directory_in = new(directory);
        }
        catch (SecurityException error)
        {
            _ = new ProgramCrash(ErrorCodes.DRIVE_RESOURCE_ACCESS_ERROR, error.Message);

            WorkDirectoriesInfo.ShowResourceDenied(Drive_Type_in!);
        } 
    }

    private void WriteDirectory(string directory)
    {
        X_Drive_in!.Value = directory;
        config_file_in.Document_in!.Save(LogFiles.DRIVES_CONFIG_FILE);
    }

    private static bool CheckDirectory(string directory_full_name)
    {
        if (directory_full_name is not "")
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

            CreateAndCheckAccessDriveDirectory(new_directory);

            return true;
        }
        else
        {
            return false;
        }
    }
}
