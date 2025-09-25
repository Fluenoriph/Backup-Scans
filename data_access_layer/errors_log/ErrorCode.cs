// * Файл "ErrorCode.cs": перечисление кодов ошибок. *

/* 
       * 101 - Ошибка доступа к уровню XML файла;
       * 201 - Ошибка доступа к ресурсу "диска";
       * 202 - Рабочая директория не найдена;
       * 211 - Ресурс "диска" недоступен;
       * 301 - Ошибка ввода значения;
       * 501 - Ошибка копирования файла;
       * 601 - Несоответствие найденной и скопированной суммы файлов.
    */

enum ErrorCode
{
    XML_ELEMENT_ACCESS_ERROR = 101,
    DRIVE_RESOURCE_ACCESS_ERROR = 201,
    DRIVE_DIRECTORY_NOT_FOUND_ERROR = 202,
    DRIVE_RESOURCE_UNAVAILABLE = 211,
    INPUT_VALUE_ERROR = 301,
    COPY_FILE_ERROR = 501,
    COPY_SUMS_FATAL_ERROR = 601
}
