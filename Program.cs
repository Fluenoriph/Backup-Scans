/*
 * Название программы: Backup "PDF" Protocols Scan Files v.2.0
 * Версия: 2.0
 * 
 * Лицензия: MIT License
 * 
 * Дата: ..... 2025 г.
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

// Создание рабочих пространств.

WorkLocations work_locations = new();

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

        if (month_index >= PeriodsNames.JANUARY_INDEX && month_index <= PeriodsNames.DECEMBER_INDEX)
        {
            MonthLoggerControl _ = new(work_locations.GetWorkDrives(), PeriodsNames.MONTHES[month_index]);

            // После успешного завершения копирования, можно запустить его заново.

            program_menu_restart = GeneralInfo.RestartOrExitProgram();
        }

        // Формат значения года: 2025.

        else if (parameter == CurrentDate.Year_in.ToString(CultureInfo.CurrentCulture))
        {
            YearLoggerControl _ = new(work_locations.GetWorkDrives());

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

        work_locations.Drives[drive_index].ChangeWorkDirectory();

        program_menu_restart = true;
    }

    // Иначе работа программы завершается.

    else
    {
        _ = new ProgramShutDown(ErrorCode.INPUT_VALUE_ERROR);
    }

} while (program_menu_restart == true);






// интерфейс ??

/*    Open .html
 * using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "C:/Users/Carma/Desktop/Мой сайт/html/math.html",
            UseShellExecute = true
        };

        Process.Start(startInfo);
    }
}
*/