class Drive
{
    public string? Directory_Name { get; set; }
    public DirectoryInfo? Directory { get; set; }

    public bool Directory_Exist
    {
        get
        {
            var dir_check = string.IsNullOrEmpty(Directory_Name);

            if (!dir_check)
            {
                Directory = new(Directory_Name!);

                if (Directory.Exists)
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
}


class DrivesControl
{
    public Drive Drive { get; set; }

    public DrivesControl(string drive_type)
    {
        // если повреждение тэга, то исключение, если просто другое имя то 'null' -- exit
        // проверить на исключение при повреждении имен дисков (тэга) -- exit
        // получаем конфигурацию

        DrivesConfigurationFile self_obj_x_drives = new(AppConstants.drives_config_file);
        var root_level = self_obj_x_drives.Document_in!.Root;

        var target_x_drive = root_level?.Element(drive_type);
        var directory = target_x_drive?.Value;

        bool directory_status;

        // проверка существования директории в системе

        Drive = new();

        do
        {
            Drive.Directory_Name = directory;
            directory_status = Drive.Directory_Exist;

            if (!directory_status)
            {
                AppInfoConsoleOut.ShowDirectoryExistFalse(drive_type);
                AppInfoConsoleOut.ShowLine();

                directory = InputNoNullText.GetRealText();

                target_x_drive!.Value = directory;
                self_obj_x_drives.Document_in.Save(AppConstants.drives_config_file);

                Console.WriteLine('\n');
                AppInfoConsoleOut.ShowInstallDirectory(drive_type);
                Console.WriteLine('\n');
            }

        } while (directory_status == false);
    }
}
