// * Файл "WorkSpaceSetuper.cs": класс для получения, проверки и установки рабочей директории. *

using System.Xml.Linq;


class WorkSpaceSetuper
{
    string? Space_Type_in { get; }
    bool Real_Directory_status_in { get; set; }
    readonly WorkSpacesConfigFile config_file_in = new(IWorkSpacesConfigFileLocation.GetPath());

    // Рабочая директория пространства.

    public string? Directory_in { get; set; }

    // Входной параметр: тип рабочего пространства.

    public WorkSpaceSetuper(string space_type)
    {
        Space_Type_in = space_type;
                  
        // Получение директории из файла

        Directory_in = config_file_in.Document_in!.Element(XMLWorkSpacesTags.ROOT)?.Element(Space_Type_in)?.Value;

        IXMLNullError<string>.CheckItem(Directory_in);
                
        // Проверка, полученной из файла настройки, директории. 

        do
        {
            Real_Directory_status_in = IRealValue.GetStatus(Directory_in) && CheckRealDirectory();

            // Если не проходит проверку, то получаем новую, в данном случае, методом ввода из консоли, проверяем и записываем в файл.

            if (!Real_Directory_status_in)
            {
                do
                {
                    WorkSpacesInfo.ShowDirectoryExistFalse(Space_Type_in!, Directory_in!);
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
                WorkSpacesInfo.ShowInstallDirectory(Space_Type_in!);
            }
            else
            {
                WorkSpacesInfo.ShowDirectoryExistFalse(Space_Type_in!, Directory_in!);
                GeneralInfo.ShowLine();
            }

        } while (Real_Directory_status_in == false);
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
        
    // * Установка новой директории. *

    bool SetupNewDirectory()
    {
        WorkSpacesInfo.ShowEnterTheDirectory();
        GeneralInfo.ShowLine();

        // Ввод из консоли

        Directory_in = InputNoNullText.GetRealText();

        // Проверка на существование. При истине, записать в файл.

        if (CheckRealDirectory())
        {
            // Чтобы изменить значение в файле, нужно заново получить всю цепочку вызовов.
                                               
            var sector_lcl = config_file_in.Document_in!.Element(XMLWorkSpacesTags.ROOT)?.Element(Space_Type_in!);

            IXMLNullError<XElement>.CheckItem(sector_lcl);
                        
            sector_lcl!.Value = Directory_in;

            config_file_in.Document_in!.Save(IWorkSpacesConfigFileLocation.GetPath());

            return true;
        }
        else
        {
            return false;
        }
    }          
}
