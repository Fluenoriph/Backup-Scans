using System.Text.RegularExpressions;


class SourceFiles
{
    private readonly FileInfo[] files;

    public SourceFiles(string drive_directory)
    {
        DirectoryInfo directory_lcl = new(drive_directory);
        files = directory_lcl.GetFiles($"*.{FilePatterns.PROTOCOL_SCAN_TYPE}");
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
