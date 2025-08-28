class DrivesControl
{
    public DirectoryInfo? Work_Directory_in { get; set; }

    public DrivesControl(string drive_type)
    {
        // если повреждение тэга, то исключение, если просто другое имя то 'null' -- exit
        // проверить на исключение при повреждении имен дисков (тэга) -- exit
        // получаем конфигурацию

        DrivesConfigurationFile x_drives_lcl = new(LogFiles.DRIVES_CONFIG_FILE);
        var root_level_lcl = x_drives_lcl.Document_in!.Root;

        var target_x_drive_lcl = root_level_lcl?.Element(drive_type);
        var directory_lcl = target_x_drive_lcl?.Value;

        bool real_directory_status;

        // проверка существования директории в системе
                
        do
        {
            real_directory_status = CheckDirectory(directory_lcl);

            if (!real_directory_status)
            {
                DirectoriesInfo.ShowDirectoryExistFalse(drive_type);
                AnyInfo.ShowLine();

                directory_lcl = InputNoNullText.GetRealText();

                target_x_drive_lcl!.Value = directory_lcl;
                x_drives_lcl.Document_in.Save(LogFiles.DRIVES_CONFIG_FILE);

                Console.WriteLine('\n');
                DirectoriesInfo.ShowInstallDirectory(drive_type);
                Console.WriteLine('\n');
            }

        } while (real_directory_status == false);
    }

    private bool CheckDirectory(string? directory_full_name)
    {
        if (!string.IsNullOrEmpty(directory_full_name))
        {
            Work_Directory_in = new(directory_full_name);
            
            if (Work_Directory_in.Exists)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
