using System.Xml.Linq;
using BackupBlock;
using DrivesControl;
using Logging;


namespace Tracing
{
    struct RgxMaskConfiguration
    {
        public static string simple_file_pattern = "^\\d{1,4}-(ф|фа|р|ра|м|ма)-";
        public static string eias_file_pattern = "^\\d{5}-\\d{2}-\\d{2}-";
    }



    //private RgxPattern SimpleFileRgx = new(RgxMaskConfiguration.simple_file_pattern);
    //private RgxPattern EiasFileRgx = new(RgxMaskConfiguration.eias_file_pattern);



    /*public void Backup()
    {
        BackupItem simple_block = new(SimpleFileRgx, Drives[0].Directory);
        simple_block.GetBackupingItems();

        ProtocolTypes protocols = new(simple_block.Result_Block);
        protocols.Calc();

        foreach (int i in protocols.Type_Sums)
        {
            Console.WriteLine(i);
        }

    }*/



}


class BlockAnalysis
    {
        
        
        public Dictionary<string, int> All_Sums_of_Protocols { get; } = [];

        





    }

}
