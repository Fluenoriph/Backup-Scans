using BackupBlock;
using DrivesControl;


const string SETTINGS_FULL_KEY = "HKEY_CURRENT_USER\\Software\\Ivan_Bogdanov\\Backup_Scans";

string simple_file_pattern = "^\\d{1,4}-(ф|фа|р|ра|м|ма)-";

//const string SOURCE_DIR = "C:\\Users\\Asus machine\\Desktop\\Files\\сканы";
//const string DESTINATION_DIR = "C:\\Users\\Asus machine\\Desktop\\Files\\result_test";


SettingsInWinRegistry x = new(SETTINGS_FULL_KEY);
var dirs = x.GetSettings();

BackupItem y = new(simple_file_pattern);

List <FileInfo> mass_files = y.GetBackupingItems("Январь", dirs[0]);

foreach (FileInfo file in mass_files)
{
    Console.WriteLine(file.FullName);
}

Console.WriteLine(y.Items_count);

ProtocolTypes z = new(mass_files);
z.CalcProtocolTypes();

foreach (KeyValuePair<string, int> kvp in z.Type_Sums)
{
    Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
}

//Console.WriteLine(z.type_sums.ToString());










