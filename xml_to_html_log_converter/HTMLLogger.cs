using Aspose.Html;
using System.Xml.Linq;


// Пока главная линия логики.

class HTMLLogger
{
    readonly XElement? sums_in;
    readonly List<string> log_data = [];

    public HTMLLogger(XElement? sums) //XElement names, DirectoryInfo log_directory)
    {
        sums_in = sums;



        string main_sums_value = GetSumValue(XmlTags.MAIN_SUMS_TAGS[0]);

        if (IRealValue.GetStatus(main_sums_value))
        {
            log_data.Add(IHTMLMainData.CreateGeneralPart("Jan", main_sums_value));
        }



        string eias_sum_value = GetSumValue(XmlTags.MAIN_SUMS_TAGS[1]);

        if (IRealValue.GetStatus(eias_sum_value))
        {
            log_data.Add(IHTMLTablePart.PutString(ProtocolTypesAndSums.MAIN_SUMS[1], eias_sum_value));
        }

        string simple_sums_value = GetSumValue(XmlTags.MAIN_SUMS_TAGS[2]);

        if (IRealValue.GetStatus(simple_sums_value))
        {
            log_data.Add(IHTMLTablePart.PutString(ProtocolTypesAndSums.MAIN_SUMS[2], simple_sums_value));
        }

        log_data.Add(@"
                      </tbody>
				</table>
            </main>
	</body>
</html>");




        using HTMLDocument log_doc = new(string.Join("", log_data), ".");
            log_doc.Save(Path.Combine("C:\\", "logsums.html"));

        


    }

    string GetSumValue(string sum_tag)
    {
        // Сначала получаем сектор текущей суммы.

        var current_sector_lcl = sums_in?.Element(sum_tag);

        if (current_sector_lcl is null)
        {
            _ = new ProgramShutDown(ErrorCode.XML_ELEMENT_ACCESS_ERROR);       // интерфейс ??, т.к. повторяется
        }

        return current_sector_lcl!.Value;
    }

}
