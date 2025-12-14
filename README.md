# Backup "PDF" Protocols Scan Files v.2.1.0

    Для визуального удобства, отчеты также создаются в формате HTML. Просмотр всех отчетов, реализован в одном файле-обозревателе.
    Файлы отчетов по месяцам генерируются программно. А файл обозревателя и стили, нужно скопировать в директорию отчетов самостоятельно.

## Информация об исходном коде
        
    Содержание:

        1) .\business_logic_layer

                BackupSumsPerMonth.cs
                    [Класс BackupSumsPerMonth]




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