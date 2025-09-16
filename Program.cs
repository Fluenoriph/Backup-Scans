/*
 * Название программы: Backup "PDF" Protocols Scan Files v.2.0
 * Версия: 2.0
 * 
 * Лицензия: MIT License
 * 
 * Дата: 08 сентября 2025 г.
 * 
 * Автор: Богданов Иван Иванович
 * Контакты: fluenoriph@gmail.com, fluenoriph@yandex.ru
 */

using InfoOut;
using InputValidate;
using System.Globalization;


// Вывод начальной информации.

GeneralInfo.ShowStarLine();
GeneralInfo.ShowAuthorInfo();
GeneralInfo.ShowLine();
GeneralInfo.ShowProgramInfo();
GeneralInfo.ShowStarLine();
Console.WriteLine('\n');

// Создание списка рабочих дисков.

List<DrivesConfiguration> work_drives = [];

foreach (string drive_type in XmlTags.DRIVE_TAGS)
{
    DrivesConfiguration drive = new(drive_type);

    // Вывод директории диска.

    WorkDirectoriesInfo.ShowDirectorySetupTrue(drive_type, drive.Work_Directory_in!);
    Console.WriteLine('\n');

    work_drives.Add(drive);
}

bool program_menu_restart = false;

do
{
    GeneralInfo.ShowLine();
    Console.WriteLine('\n');

    GeneralInfo.ShowProgramMenu();

    // Ввод параметра функции меню.

    var parameter = InputNoNullText.GetRealText();

    // Если введено число, то запускается резервное копирование.

    if (int.TryParse(parameter, out int _))
    {
        // Индекс месяца в списке.

        int month_index = Convert.ToInt32(parameter, CultureInfo.CurrentCulture) - 1;

        // Правильные цифры: 1 - 12.

        if (month_index >= PeriodsNames.JANUARY_INDEX && month_index <= PeriodsNames.DECEMBER_INDEX)
        {
            MonthBackupProcess _ = new(work_drives, PeriodsNames.MONTHES[month_index]);

            // Отделять модули лога и парсера.


            // После успешного завершения копирования, можно запустить его заново.

            program_menu_restart = GeneralInfo.RestartOrExitProgram();
        }
        // Формат значения года: 2025.

        else if (parameter == CurrentDate.Year_in.ToString(CultureInfo.CurrentCulture))
        {
            YearBackupProcess _ = new(work_drives);

            program_menu_restart = GeneralInfo.RestartOrExitProgram();
        }
        // Если введено неверное число, то работа программы завершается.

        else
        {
            _ = new ProgramShutDown(ErrorCode.INPUT_VALUE_ERROR);
        }
    }
    // Если введена верная буква, то запускается функция изменения директорий.

    else if (parameter is Symbols.CHANGE_DIRECTORY_FUNCTION)
    {
        Console.WriteLine('\n');

        WorkDirectoriesInfo.ShowEnterDirectoryType();
        var drive_index = DriveIndex.Index_in;

        if (drive_index == Symbols.NOT_DRIVE_INDEX)
        {
            _ = new ProgramShutDown(ErrorCode.INPUT_VALUE_ERROR);
        }

        work_drives[drive_index].ChangeWorkDirectory();

        program_menu_restart = true;
    }
    // Иначе работа программы завершается.

    else
    {
        _ = new ProgramShutDown(ErrorCode.INPUT_VALUE_ERROR);
    }

} while (program_menu_restart == true);

// 2 новый класса для отдельных задач:
// ProtocolNamesComputing(eias files, simple files)
// BackupSums(или предыдущий объект или отдельные списки в параметре)
// Другой интерфейс для логгера за месяц и за год.





