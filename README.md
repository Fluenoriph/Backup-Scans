# Backup "PDF" Protocols Scan Files v.2.0

    Программа для резервного копирования сканированных копий протоколов ЕИАС Роспотребнадзора и обычных внутренних протоколов по "физическим факторам" в формате PDF.
    Периодичность копирования, реализованная в программе: за месяц или за год.

    Общий вид шаблона имени протокола.
        ЕИАС: 12345-01-25-25.01.2025
        Обычный: 1-ф-25.01.2025; 20-фа-25.01.2025; 35-р-25.01.2025; 400-ра-25.01.2025; 510-м-25.01.2025; 625-ма-25.01.2025

    Реализован рассчет сумм всех типов протоколов. Для обычных протоколов, вычисляются полностью все варианты, а также пропущенные и неизвестные протоколы.
    Как результат, создается отчет, со всеми суммами и именами протоколов. Штатно, отчет создается в формате XML.
    По умолчанию, файлы с одинаковыми именами перезаписываются в резервном хранилище.

## Информация об исходном коде
    
    Содержание:
        1. BackupProcess.cs
            * Класс BaseBackupProcess
            * Класс MonthBackupProcess
            * Класс YearBackupProcess
        2. CurrentDate.cs
            * Структура CurrentDate
        3. DrivesConfiguration.cs
            * Класс ConfigurationFile
            * Класс DrivesConfiguration
        4. ErrorLog.cs
            * Класс BaseErrorReporter
            * Перечисление ErrorCode            
            * Класс ProgramCrash
            * Класс ProgramShutDown
        5. ExtremeNumbers.cs
            * Класс BaseExtremeNumbers
            * Класс MaximumNumbers
            * Класс MinimumNumbers
        6. ISumsTableCreator.cs
            * Интерфейс ISumsTableCreator
        7. IXmlLevelCreator.cs
            * Интерфейс IXmlLevelCreator
        8. LogFiles.cs
            * Класс MonthLogFile
            * Класс YearLogFile
        9. MonthBackupSums.cs
            * Класс MonthBackupSums
       10. NameSorter.cs
            * Класс BaseNameSorter
            * Класс EIASSort
            * Класс SimpleSort
       11. NumberConverter.cs
            * Класс BaseNumberConverter
            * Класс EIASConvert
            * Класс SimpleConvert
       12. Program.cs
            * Инструкция верхнего уровня
       13. ProgramData.cs
           * Структура DrivesConfigFileLocation
           * Структура FilePatterns
           * Структура LogFilesNames
           * Структура PeriodsNames
           * Структура ProtocolTypesAndSums
           * Структура Symbols
           * Структура XmlTags
       14. ProgramInfoConsoleOut.cs
            а) Пространство имен InfoOut
                * Класс BackupInfo
                * Класс GeneralInfo
                * Класс ParameterTemplates
                * Класс WorkDirectoriesInfo
            б) Пространство имен InputValidate
                * Класс DriveIndex
                * Класс InputNoNullText
            в) Пространство имен ResultLogOut
                * Класс FullLogPrinter
       15. ResultLoggers.cs
            * Класс BaseSumsData
            * Класс MonthLogger
            * Класс YearLogger
       16. SimpleProtocolNames.cs
            * Класс SimpleProtocolNames
       17. SourceFiles.cs
            * Класс SourceFiles
       18. TotalLogSumsToYearCalculator.cs
            * Класс TotalLogSumsToYearCalculator
       19. BaseXmlDataFile.cs
            * Класс BaseXmlDataFile
    
### Положение об именовании

    1. Именованные константы. Данные, которые не изменяются в программе: VARIABLE_NAME
    2. Локальные переменные: variable_name_lcl (кроме внутренних переменных цикла и параметров методов и классов)
    3. Переменные статуса: variable_name_status
    4. Поля класса: variable_name_in
    5. Пользовательские объекы: self_obj_variable_name
    6. Классы и методы, пространства имен: ClassName
    7. Базовые классы: BaseClassName 

    Примечание. 
        К инструкции верхнего уровня это не относится.