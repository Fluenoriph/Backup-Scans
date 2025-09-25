// * Файл "WorkSpaceSetuper.cs": класс для получения, проверки и установки рабочей директории. *

using System.Xml.Linq;
using InfoOut;
using InputValidate;


class WorkSpaceSetuper
{
    string? Space_Type_in { get; }
    
    bool Real_Directory_status_in { get; set; }
    public string? Directory_in { get; set; }

    // Входной параметр: тип рабочего пространства.

    public WorkSpaceSetuper(string space_type)
    {
        Space_Type_in = space_type;
                  
        // Получение директории из файла

        Directory_in = GetConfiguration().Document_in!.Element(XMLLogTags.DRIVES_DIRECTORIES_TAG)?.Element(Space_Type_in)?.Value;

        IXMLNullError<string>.CheckItem(Directory_in);
                
        // Проверка, полученной из файла настройки, директории. 

        do
        {
            Real_Directory_status_in = CheckNoneDirectoryValue() && CheckRealDirectory();

            // Если не проходит проверку, то получаем новую, в данном случае, методом ввода из консоли, проверяем и записываем в файл.

            if (!Real_Directory_status_in)
            {
                do
                {
                    WorkDirectoriesInfo.ShowDirectoryExistFalse(Space_Type_in!, Directory_in!);
                    GeneralInfo.ShowLine();

                    Real_Directory_status_in = SetupNewDirectory();

                    Console.WriteLine('\n');

                } while (Real_Directory_status_in == false);
            }

        } while (Real_Directory_status_in == false);
    }

    // * Изменение уже установленной директории. *

    public void ChangeWorkDirectory()
    {
        do
        {
            // Ввод из консоли, если действительная, то вывод сообщения об успехе, если нет, то повторяем заново.

            Real_Directory_status_in = SetupNewDirectory();

            if (Real_Directory_status_in)
            {
                WorkDirectoriesInfo.ShowInstallDirectory(Space_Type_in!);
            }
            else
            {
                WorkDirectoriesInfo.ShowDirectoryExistFalse(Space_Type_in!, Directory_in!);
                GeneralInfo.ShowLine();
            }

        } while (Real_Directory_status_in == false);
    }

    // * Проверка на "пустую" строку. *

    bool CheckNoneDirectoryValue()
    {
        if (Directory_in is not "")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // * Проверка на существование в системе. *

    bool CheckRealDirectory()
    {
        if (Directory.Exists(Directory_in))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // * Файл настроек рабочих пространств. *

    static WorkSpacesConfigFile GetConfiguration()
    {
        return new(DrivesConfigFileLocation.full_program_path);
    }

    // * Установка новой директории. *

    bool SetupNewDirectory()
    {
        WorkDirectoriesInfo.ShowEnterTheDirectory();
        GeneralInfo.ShowLine();

        // Ввод из консоли

        Directory_in = InputNoNullText.GetRealText();

        // Проверка на существование. При истине, записать в файл.

        if (CheckRealDirectory())
        {
            // Чтобы изменить значение в файле, нужно заново получить всю цепочку вызовов.

            WorkSpacesConfigFile self_obj_config_file_lcl = GetConfiguration();
                        
            var sector_lcl = self_obj_config_file_lcl.Document_in!.Element(XMLLogTags.DRIVES_DIRECTORIES_TAG)?.Element(Space_Type_in!);

            IXMLNullError<XElement>.CheckItem(sector_lcl);
                        
            sector_lcl!.Value = Directory_in;
            
            self_obj_config_file_lcl.Document_in!.Save(DrivesConfigFileLocation.full_program_path);

            return true;
        }
        else
        {
            return false;
        }
    }          
}
