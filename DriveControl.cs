using System.Xml.Linq;
using InputValidate;
using InfoOut;
using System.Security;


// * Класс файла настройки рабочих директорий. *

class DrivesConfigurationFile(string file_path) : XmlDataFile(file_path)
{
    private protected override XElement Root_Sector_in { get; } = IXmlLevelCreator.Create(XmlTags.DRIVES_CONFIG_TAG, XmlTags.DRIVE_TAGS);
}


// * Класс для получения, проверки и установки рабочей директории ("диска"). *

class DriveControl
{
    public string Work_Directory_in { get; set; }  

    private string? Drive_Type_in { get; }
    private readonly DrivesConfigurationFile config_file_in = new(LogFiles.DRIVES_CONFIG_FILE);
    private XElement? Drive_Directory_Sector_in { get; }
    private bool Real_Directory_status { get; set; }

    // Входной параметр: имя типа "диска".

    public DriveControl(string drive_type)
    {        
        Drive_Directory_Sector_in = config_file_in.Document_in!.Element(XmlTags.DRIVES_CONFIG_TAG)?.Element(drive_type);
                
        if (Drive_Directory_Sector_in is null)
        {
            _ = new ProgramShutDown(ErrorCodes.XML_ELEMENT_ACCESS_ERROR);
        }

        /* Проверка, полученной из файла настройки, директории. Если она существует, то устанавливаем рабочую директорию.
         * Если нет, то получаем новую, в данном случае, методом ввода из консоли, и записываем в файл. */

        Work_Directory_in = Drive_Directory_Sector_in!.Value;
                
        

        
    }

    public void CheckWorkDirectory() // drive local ??
    {
        do
        {
            Real_Directory_status = CheckNoneDirectoryValue() && CheckRealDirectory();                        // real local ??
                        
            if (!Real_Directory_status)
            {
                do
                {
                    WorkDirectoriesInfo.ShowDirectoryExistFalse(Drive_Type_in!, Work_Directory_in);
                    GeneralInfo.ShowLine();

                    Real_Directory_status = PrepareAndWriteDirectory();

                    Console.WriteLine('\n');

                } while (Real_Directory_status == false); 
            }

        } while (Real_Directory_status == false);
    }

    private bool PrepareAndWriteDirectory()
    {
        WorkDirectoriesInfo.ShowEnterTheDirectory();
        GeneralInfo.ShowLine();

        Work_Directory_in = InputNoNullText.GetRealText();

        if (CheckRealDirectory())
        {
            Drive_Directory_Sector_in!.Value = Work_Directory_in;
            config_file_in.Document_in!.Save(LogFiles.DRIVES_CONFIG_FILE);

            return true;
        }
        else
        {
            return false;
        }
    }


    /*private void CreateAndCheckAccessDriveDirectory(string directory)
    {
        try
        {
            Work_Directory_in = new(directory);
        }
        catch (SecurityException error)
        {
            //_ = new ProgramCrash(ErrorCodes.DRIVE_RESOURCE_ACCESS_ERROR, error.Message);  in backup process

            WorkDirectoriesInfo.ShowResourceDenied(Drive_Type_in!);
        } 
    }*/

    // * Запись в файл настройки. *

   
    private bool CheckRealDirectory()
    {
        if (Directory.Exists(Work_Directory_in))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckNoneDirectoryValue()
    {
        if (Work_Directory_in is not "")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ChangeWorkDirectory()
    {
        do
        {
            Real_Directory_status = PrepareAndWriteDirectory();

            if (Real_Directory_status)
            {
                Console.WriteLine('\n');
                WorkDirectoriesInfo.ShowInstallDirectory(Drive_Type_in!);

            }
            else
            {
                WorkDirectoriesInfo.ShowDirectoryExistFalse(Drive_Type_in!, Work_Directory_in);
                GeneralInfo.ShowLine();
            }

        } while (Real_Directory_status == false);
    }
}
