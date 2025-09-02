using System.Text.RegularExpressions;


class SourceFiles
{
    private readonly FileInfo[]? files;  

    public SourceFiles(DirectoryInfo drive_directory) // new dirinfo
    {
        try
        {
            files = drive_directory.GetFiles($"*.{FilePatterns.PROTOCOL_SCAN_TYPE}");
        }
        catch (DirectoryNotFoundException error)
        {
            _ = new ProgramShutDown(ErrorCodes.DRIVE_DIRECTORY_NOT_FOUND_ERROR, error.Message);
        }
    }

    public List<FileInfo>? GrabMatchedFiles(Regex pattern)
    {
        IEnumerable<FileInfo> backup_block_lcl = from file in files
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
