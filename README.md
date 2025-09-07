# Backup "PDF" Protocols Scan Files v.2.0

    ��������� ��� ���������� ����������� ������������� ����� ���������� ���� ���������������� � ������� ���������� ���������� �� "���������� ��������" � ������� PDF.
    ������������� �����������, ������������� � ���������: �� ����� ��� �� ���.

    ����� ��� ������� ����� ���������.
        ����: 12345-01-25-25.01.2025
        �������: 1-�-25.01.2025; 20-��-25.01.2025; 35-�-25.01.2025; 400-��-25.01.2025; 510-�-25.01.2025; 625-��-25.01.2025

    ���������� ������� ���� ���� ����� ����������. ��� ������� ����������, ����������� ��������� ��� ��������, � ����� ����������� � ����������� ���������.
    ��� ���������, ��������� �����, �� ����� ������� � ������� ����������. ������, ����� ��������� � ������� XML.
    �� ���������, ����� � ����������� ������� ���������������� � ��������� ���������.

## ���������� �� �������� ����
    
    ����������:
        1. BackupProcess.cs
            * ����� BaseBackupProcess
            * ����� MonthBackupProcess
            * ����� YearBackupProcess
        2. CurrentDate.cs
            * ��������� CurrentDate
        3. DrivesConfiguration.cs
            * ����� ConfigurationFile
            * ����� DrivesConfiguration
        4. ErrorLog.cs
            * ����� BaseErrorReporter
            * ������������ ErrorCode            
            * ����� ProgramCrash
            * ����� ProgramShutDown
        5. ExtremeNumbers.cs
            * ����� BaseExtremeNumbers
            * ����� MaximumNumbers
            * ����� MinimumNumbers
        6. ISumsTableCreator.cs
            * ��������� ISumsTableCreator
        7. IXmlLevelCreator.cs
            * ��������� IXmlLevelCreator
        8. LogFiles.cs
            * ����� MonthLogFile
            * ����� YearLogFile
        9. MonthBackupSums.cs
            * ����� MonthBackupSums
       10. NameSorter.cs
            * ����� BaseNameSorter
            * ����� EIASSort
            * ����� SimpleSort
       11. NumberConverter.cs
            * ����� BaseNumberConverter
            * ����� EIASConvert
            * ����� SimpleConvert
       12. Program.cs
            * ���������� �������� ������
       13. ProgramData.cs
           * ��������� DrivesConfigFileLocation
           * ��������� FilePatterns
           * ��������� LogFilesNames
           * ��������� PeriodsNames
           * ��������� ProtocolTypesAndSums
           * ��������� Symbols
           * ��������� XmlTags
       14. ProgramInfoConsoleOut.cs
            �) ������������ ���� InfoOut
                * ����� BackupInfo
                * ����� GeneralInfo
                * ����� ParameterTemplates
                * ����� WorkDirectoriesInfo
            �) ������������ ���� InputValidate
                * ����� DriveIndex
                * ����� InputNoNullText
            �) ������������ ���� ResultLogOut
                * ����� FullLogPrinter
       15. ResultLoggers.cs
            * ����� BaseSumsData
            * ����� MonthLogger
            * ����� YearLogger
       16. SimpleProtocolNames.cs
            * ����� SimpleProtocolNames
       17. SourceFiles.cs
            * ����� SourceFiles
       18. TotalLogSumsToYearCalculator.cs
            * ����� TotalLogSumsToYearCalculator
       19. BaseXmlDataFile.cs
            * ����� BaseXmlDataFile
    
### ��������� �� ����������

    1. ����������� ���������. ������, ������� �� ���������� � ���������: VARIABLE_NAME
    2. ��������� ����������: variable_name_lcl (����� ���������� ���������� ����� � ���������� ������� � �������)
    3. ���������� �������: variable_name_status
    4. ���� ������: variable_name_in
    5. ���������������� ������: self_obj_variable_name
    6. ������ � ������, ������������ ����: ClassName
    7. ������� ������: BaseClassName 

    ����������. 
        � ���������� �������� ������ ��� �� ���������.