// * Файл SourceFiles.cs: класс для получения файлов сканов из исходной директории (диска). *

using System.Text.RegularExpressions;


class SourceFiles
{
    readonly FileInfo[]? files_in;  

    // Входной параметр: исходная директория.

    public SourceFiles(DirectoryInfo source_drive_directory) 
    {
        // Получаем все файлы "PDF", которые есть в директории.

        try
        {
            files_in = source_drive_directory.GetFiles($"*.{FilePatterns.PROTOCOL_SCAN_FILE_TYPE}");
        }
        catch (DirectoryNotFoundException error)
        {
            _ = new ProgramShutDown(ErrorCode.DRIVE_DIRECTORY_NOT_FOUND_ERROR, error.Message);
        }
    }

    // * Поиск протоколов по паттерну, из всех найденных ранее по типу файла. *

    public List<FileInfo>? GrabMatchedFiles(Regex pattern)
    {
        IEnumerable<FileInfo> backup_block_lcl = from file in files_in
                                                 where pattern.IsMatch(file.Name)
                                                 select file;

        if (backup_block_lcl.Any())
        {
            return [.. backup_block_lcl];
        }
        else
        {
            return null;
        }
    }
}
