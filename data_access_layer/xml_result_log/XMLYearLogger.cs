// * Файл "XmlYearLogger": логгер отчета за год. *

class XmlYearLogger : BaseXmlSumsData
{
    // Параметры: "file" - файл годового отчета, "all_protocols_sums" - главные суммы за год, "simple_protocols_sums" - суммы протоколов по физическим факторам за год. 

    public XmlYearLogger(YearLogFile file, Dictionary<string, int> all_protocols_sums, Dictionary<string, int> simple_protocols_sums)
    {
        Sums_Sector_in = file.Document_in!.Element(XMLLogTags.SUMS);

        // Запись и сохранение.

        WriteSums(XMLLogTags.MAIN_SUMS, all_protocols_sums, ProtocolTypesAndSums.MAIN_SUMS);
        WriteSums(XMLLogTags.SIMPLE_PROTOCOLS_SUMS, simple_protocols_sums, ProtocolTypesAndSums.UNITED_SIMPLE_TYPE_SUMS);

        file.Document_in.Save(file.Filename_in);
    }
}
