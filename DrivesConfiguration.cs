using System.Xml.Linq;
using InputValidate;
using InfoOut;


// * Класс файла настройки рабочих директорий. *

class DrivesConfigurationFile(string file_path) : XmlDataFile(file_path)
{
    private protected override XElement Root_Sector_in { get; } = IXmlLevelCreator.Create(XmlTags.DRIVES_CONFIG_TAG, XmlTags.DRIVE_TAGS);
}


// * Класс для получения, проверки и установки рабочей директории ("диска"). *

class DrivesConfiguration
{
    private string? Drive_Type_in { get; }
    
    private bool Real_Directory_status { get; set; }
    public string? Work_Directory_in { get; set; }

    // Входной параметр: имя типа "диска".

    public DrivesConfiguration(string drive_type)
    {
        Drive_Type_in = drive_type;
                  
        // Получение директории из файла

        Work_Directory_in = GetConfiguration().Document_in!.Element(XmlTags.DRIVES_CONFIG_TAG)?.Element(Drive_Type_in)?.Value;

        if (Work_Directory_in is null)
        {
            _ = new ProgramShutDown(ErrorCode.XML_ELEMENT_ACCESS_ERROR);
        }

        // Проверка, полученной из файла настройки, директории. 

        do
        {
            Real_Directory_status = CheckNoneDirectoryValue() && CheckRealDirectory();

            // Если не проходит проверку, то получаем новую, в данном случае, методом ввода из консоли, проверяем и записываем в файл.

            if (!Real_Directory_status)
            {
                do
                {
                    WorkDirectoriesInfo.ShowDirectoryExistFalse(Drive_Type_in!, Work_Directory_in!);
                    GeneralInfo.ShowLine();

                    Real_Directory_status = SetupNewDirectory();

                    Console.WriteLine('\n');

                } while (Real_Directory_status == false);
            }

        } while (Real_Directory_status == false);
    }

    // * Изменение уже установленной директории. *

    public void ChangeWorkDirectory()
    {
        do
        {
            // Ввод из консоли, если действительная, то вывод сообщения об успехе, если нет, то повторяем заново.

            Real_Directory_status = SetupNewDirectory();

            if (Real_Directory_status)
            {
                WorkDirectoriesInfo.ShowInstallDirectory(Drive_Type_in!);
            }
            else
            {
                WorkDirectoriesInfo.ShowDirectoryExistFalse(Drive_Type_in!, Work_Directory_in!);
                GeneralInfo.ShowLine();
            }

        } while (Real_Directory_status == false);
    }

    // * Проверка на "пустую" строку. *

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

    // * Проверка на существование в системе. *

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

    // * Файл настроек дисков. *

    private static DrivesConfigurationFile GetConfiguration()
    {
        return new(LogFiles.DRIVES_CONFIG_FILE);
    }

    // * Установка новой директории. *

    private bool SetupNewDirectory()
    {
        WorkDirectoriesInfo.ShowEnterTheDirectory();
        GeneralInfo.ShowLine();

        // Ввод из консоли

        Work_Directory_in = InputNoNullText.GetRealText();

        // Проверка на существование. При истине, записать в файл.

        if (CheckRealDirectory())
        {
            // Чтобы изменить значение в файле, нужно заново получить всю цепочку вызовов.

            DrivesConfigurationFile config_file_lcl = GetConfiguration();
                        
            var drive_sector_lcl = config_file_lcl.Document_in!.Element(XmlTags.DRIVES_CONFIG_TAG)?.Element(Drive_Type_in!);
              
            if (drive_sector_lcl is null)
            {
                _ = new ProgramShutDown(ErrorCode.XML_ELEMENT_ACCESS_ERROR);
            }

            drive_sector_lcl!.Value = Work_Directory_in;
            
            config_file_lcl.Document_in!.Save(LogFiles.DRIVES_CONFIG_FILE);

            return true;
        }
        else
        {
            return false;
        }
    }          
}
