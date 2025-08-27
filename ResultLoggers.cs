using BackupBlock;
using System.Globalization;
using System.Xml.Linq;


abstract class SumsData
{
    private protected XElement? Sums_Sector_in { get; set; }

    private protected void WriteSums(List<string> tags, Dictionary<string, int>? sums = null, List<string>? names = null)
    {
        for (int sum_index = 0; sum_index < tags.Count; sum_index++)
        {
            var sum_lcl = Sums_Sector_in?.Element(tags[sum_index]);   // exception test

            if (sum_lcl is not null)
            {
                if ((sums is not null) && (names is not null))
                {
                    sum_lcl.Value = sums[names[sum_index]].ToString(CultureInfo.CurrentCulture);
                }
                else
                {
                    sum_lcl.Value = '0'.ToString();     // 1 char_null
                }
            }
            else
            {
                // error  ошибка доступа к элементу xml файла -- item.tostring ??  101
            }
        }
    }
}


class YearLogger : SumsData
{
    public YearLogger(YearLogFile file, Dictionary<string, int> all_protocols_sums, Dictionary<string, int> simple_protocols_sums)
    {
        Sums_Sector_in = file.Document_in.Root;

        WriteSums(XmlTags.OTHERS_SUMS_TAGS, all_protocols_sums, ProtocolTypesSums.OTHERS_SUMS);
        WriteSums(XmlTags.SIMPLE_SUMS_TAGS, simple_protocols_sums, ProtocolTypesSums.UNITED_SIMPLE_TYPE_NAMES);

        file.Document_in.Save(file.Filename_in);
    }
}


class MonthLogger : SumsData
{
    private XElement? Protocol_Names_Sector_in { get; set; }

    public MonthLogger(MonthLogFile file, string month_name, MonthBackupSums backup_sums, List<FileInfo>? eias_files)
    {
        var month_sector_lcl = file.GetMonthData(month_name);

        Sums_Sector_in = month_sector_lcl?.Element(XmlTags.SUMS_TAG);
        Protocol_Names_Sector_in = month_sector_lcl?.Element(XmlTags.PROTOCOL_NAMES_TAG);

        WriteSums(XmlTags.OTHERS_SUMS_TAGS, backup_sums.All_Protocols, ProtocolTypesSums.OTHERS_SUMS);

        if (backup_sums.All_Protocols[ProtocolTypesSums.OTHERS_SUMS[1]] != 0)
        {
            EIASConvert number_convert_self_obj = new();
            EIASSort name_sort_self_obj = new();

            WriteNames(XmlTags.OTHERS_SUMS_TAGS[1], name_sort_self_obj.Sorting(number_convert_self_obj.ConvertToNumbers(eias_files!), eias_files!));
        }
        else
        {
            WriteNames(XmlTags.OTHERS_SUMS_TAGS[1]);
        }

        if (backup_sums.All_Protocols[ProtocolTypesSums.OTHERS_SUMS[2]] != 0)
        {
            WriteSums(XmlTags.SIMPLE_SUMS_TAGS, backup_sums.Simple_Protocols_Sums, ProtocolTypesSums.UNITED_SIMPLE_TYPE_NAMES);

            foreach (string name in ProtocolTypesSums.TYPES_FULL_NAMES)
            {
                var target_tag_lcl = XmlTags.TYPE_TAGS[ProtocolTypesSums.TYPES_FULL_NAMES.IndexOf(name)];

                if (backup_sums.self_obj_names!.Sorted_Names.TryGetValue(name, out List<string>? value))
                {
                    WriteNames(target_tag_lcl, value);
                }
                else
                {
                    WriteNames(target_tag_lcl);
                }
            }

            WriteNames(XmlTags.SIMPLE_SUMS_TAGS[11], backup_sums.self_obj_names!.Missed_Protocols);
            WriteNames(XmlTags.SIMPLE_SUMS_TAGS[12], backup_sums.self_obj_names!.Unknown_Protocols);
        }
        else
        {
            WriteSums(XmlTags.SIMPLE_SUMS_TAGS);

            foreach (var tag in XmlTags.TYPE_TAGS)
            {
                WriteNames(tag);
            }

            WriteNames(XmlTags.SIMPLE_SUMS_TAGS[11]);
            WriteNames(XmlTags.SIMPLE_SUMS_TAGS[12]);
        }

        file.Document_in.Save(file.Filename_in);
    }

    private void WriteNames(string tag, List<string>? names = null)
    {
        var names_lcl = Protocol_Names_Sector_in?.Element(tag);

        if (names_lcl is not null)
        {
            if (names is not null)
            {
                names_lcl.Value = string.Join(", ", names);
            }
            else
            {
                names_lcl.Value = string.Empty;
            }
        }
        else
        {
            // xml error   101
        }
    }
}


class YearLogResultCalculate
{
    private int sum_count_in;

    private readonly MonthLogFile month_log_file_in;
    private readonly YearLogFile year_log_file_in;

    private readonly List<int> calculated_sums_in = [];

    public YearLogResultCalculate(MonthLogFile month_log_file, YearLogFile year_log_file)
    {
        month_log_file_in = month_log_file;
        year_log_file_in = year_log_file;

        foreach (string sum_tag in SumsSector.tags)
        {
            foreach (string month_name in PeriodsNames.MONTHES)
            {
                AddSum(month_name, sum_tag);
            }

            WriteYearSum(sum_tag);
                        
            calculated_sums_in.Add(sum_count_in);
        }

        year_log_file.Document_in.Save(year_log_file.Filename_in);
    }

    private void AddSum(string month_name, string sum_tag)
    {
        var sum_value_lcl = month_log_file_in.GetMonthData(month_name)?.Element(XmlTags.SUMS_TAG)?.Element(sum_tag)?.Value;

        bool real_value = (sum_value_lcl is not null) && (sum_value_lcl is not "0") && (sum_value_lcl is not "");   // 2 char null

        if (real_value)
        {
            sum_count_in += Convert.ToInt32(sum_value_lcl!, CultureInfo.CurrentCulture);
        }
        else
        {
            // err 101
        }
    }

    private void WriteYearSum(string sum_tag)
    {
        var sum_value_lcl = year_log_file_in.Document_in.Root?.Element(sum_tag);

        if (sum_value_lcl is not null)
        {
            sum_value_lcl.Value = sum_count_in.ToString(CultureInfo.CurrentCulture);
        }
        else
        {
            // xml error exit ??   101
        }
    }

    public (Dictionary<string, int>, Dictionary<string, int>?) GetYearSums()
    {
        var all_protocol_sums_lcl = IGeneralSums.CreateTable();

        for (int sum_index = 0; sum_index < all_protocol_sums_lcl.Count; sum_index++)
        {
            all_protocol_sums_lcl[ProtocolTypesSums.OTHERS_SUMS[sum_index]] = calculated_sums_in.GetRange(0, 3)[sum_index];
        }

        Dictionary<string, int>? simple_protocol_sums_lcl = null;

        if (calculated_sums_in[2] != 0)
        {
            simple_protocol_sums_lcl = ISimpleProtocolsSums.CreateTable();

            for (int sum_index = 0; sum_index < simple_protocol_sums_lcl.Count; sum_index++)
            {
                simple_protocol_sums_lcl[ProtocolTypesSums.UNITED_SIMPLE_TYPE_NAMES[sum_index]] = calculated_sums_in.GetRange(3, 13)[sum_index];
            }
        }

        return (all_protocol_sums_lcl, simple_protocol_sums_lcl);
    }
}
