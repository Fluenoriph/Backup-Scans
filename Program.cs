/*
 * Название программы: Backup "PDF" Protocols Scan Files v.2.1.0
 * Версия: 2.1.0
 * 
 * Лицензия: MIT License
 * 
 * Дата: Декабрь 2025 г.
 * 
 * Автор: Богданов Иван Иванович
 * Контакты: fluenoriph@gmail.com, fluenoriph@yandex.ru
 */

using System.Globalization;


// Вывод начальной информации.

GeneralInfo.ShowStarLine();
GeneralInfo.ShowAuthorInfo();
GeneralInfo.ShowLine();
GeneralInfo.ShowProgramInfo();
GeneralInfo.ShowStarLine();
Console.WriteLine('\n');

// Создание рабочих пространств.

WorkSpacesCreator work_spaces = new();

// Перезапускаемое меню.

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

        if (month_index >= Periods.JANUARY_INDEX && month_index <= Periods.DECEMBER_INDEX)
        {
            _ = new MonthAppControl(work_spaces.GetWorkSpaces(), Periods.MONTHES[month_index]);

            // После успешного завершения копирования, можно запустить его заново.

            program_menu_restart = GeneralInfo.RestartOrExitProgram();
        }

        // Формат значения года: 2025.

        else if (parameter == CurrentDate.Year_in.ToString(CultureInfo.CurrentCulture))
        {
            _ = new YearAppControl(work_spaces.GetWorkSpaces());

            program_menu_restart = GeneralInfo.RestartOrExitProgram();
        }

        // Если введено неверное число, то работа программы завершается.

        else
        {
            _ = new ProgramShutDown(ErrorCode.INPUT_VALUE_ERROR);
        }
    }

    // Если введена верная буква, то запускается функция изменения директорий.

    else if (parameter is ConsoleSymbols.CHANGE_DIRECTORY_FUNCTION)
    {
        Console.WriteLine('\n');

        WorkSpacesInfo.ShowEnterDirectoryType();
        var drive_index = WorkSpaceIndexConverter.Index_in;

        if (drive_index == (int)WorkSpaceIndex.NOT_SPACE_INDEX)
        {
            _ = new ProgramShutDown(ErrorCode.INPUT_VALUE_ERROR);
        }

        work_spaces.Spaces_in[drive_index].ChangeWorkDirectory();

        program_menu_restart = true;
    }

    // Иначе работа программы завершается.

    else
    {
        _ = new ProgramShutDown(ErrorCode.INPUT_VALUE_ERROR);
    }

} while (program_menu_restart == true);
