using System.Text.RegularExpressions;
using TextData;


namespace BackupBlock
{
    interface IGeneralSums
    {
        static Dictionary<string, int> CreateTable()
        {
            Dictionary<string, int> sums = [];

            AppConstants.others_sums.ForEach(item => sums.Add(item, 0));

            return sums;
        }
    }


    interface ISimpleProtocolsSums
    {
        static Dictionary<string, int> CreateTable()
        {
            Dictionary<string, int> sums = [];

            AppConstants.united_simple_type_names.ForEach(name => sums.Add(name, 0));

            return sums;
        }
    }


    class SourceFiles
    {
        private readonly FileInfo[] files;
        
        public SourceFiles(string drive_directory)
        {
            DirectoryInfo directory = new(drive_directory);
            files = directory.GetFiles($"*.{AppConstants.protocol_file_type}");
        }

        public List<FileInfo>? GrabMatchedFiles(Regex pattern)
        {
            IEnumerable<FileInfo> backup_block = from file in files
                                                 where pattern.IsMatch(file.Name)
                                                 select file;

            if (backup_block.Any())
            {
                return [.. backup_block];
            }
            else
            {
                return null;
            }
        }
    }


    abstract class NumberConverter
    {
        abstract private protected string Number_Pattern { get; }
        abstract private protected string NumberName(Match match);

        public List<int> ConvertToNumbers(List<FileInfo> files)
        {
            List<int> numbers = [];

            foreach (FileInfo protocol in files)
            {
                Match match = Regex.Match(protocol.Name, Number_Pattern);

                if (match.Success)
                {
                    numbers.Add(Convert.ToInt32(NumberName(match)));             
                }
            }

            numbers.Sort();
            return numbers;
        }
    }

        
    class SimpleConvert : NumberConverter
    {
        private protected override string Number_Pattern { get; } = AppConstants.simple_number_pattern;

        private protected override string NumberName(Match match)
        {
            return match.Groups[1].Value;
        }
    }


    class EIASConvert : NumberConverter
    {
        private protected override string Number_Pattern { get; } = AppConstants.eias_number_pattern;

        private protected override string NumberName(Match match)
        {
#pragma warning disable CA1307
            return match.Groups[1].Value.Replace(AppConstants.line.ToString(), string.Empty);
#pragma warning restore CA1307
        }
    }


    abstract class NameSorter
    {
        abstract private protected string GetNumberName(int number);

        public List<string> Sorting(List<int> numbers, List<FileInfo> files)
        {
            List<string> names = [];

            foreach (int number in numbers)
            {
                foreach (FileInfo protocol in files)
                {
                    if (protocol.Name.StartsWith(GetNumberName(number)))
                    {
                        names.Add(protocol.Name);
                        break;
                    }
                }
            }
            return names;
        }
    }


    class SimpleSort : NameSorter
    {
        private protected override string GetNumberName(int number)
        {
            return number.ToString();
        }
    }


    class EIASSort : NameSorter
    {
        private protected override string GetNumberName(int number)
        {
            string number_name = number.ToString();

            number_name = number_name.Insert(5, AppConstants.line.ToString());
            number_name = number_name.Insert(8, AppConstants.line.ToString());

            return number_name;
        }
    }
                     

    abstract class ExtremeNumbers(Dictionary<string, List<int>> protocol_type_numbers) 
    {
        private readonly Dictionary<string, int> numbers = [];
        private protected abstract int GetExtremeNumber(List<int> current_numbers);

        public Dictionary<string, int> Numbers
        {
            get
            {
                foreach (var item in protocol_type_numbers)
                {
                    numbers.Add(item.Key, GetExtremeNumber(item.Value));                                        
                }

                return numbers;
            }
        }
    }


    class MaximumNumbers(Dictionary<string, List<int>> protocol_type_numbers) : ExtremeNumbers(protocol_type_numbers)
    {
        private protected override int GetExtremeNumber(List<int> current_numbers)
        {
            return current_numbers.Last();
        }
    }


    class MinimumNumbers(Dictionary<string, List<int>> protocol_type_numbers) : ExtremeNumbers(protocol_type_numbers)
    {
        private protected override int GetExtremeNumber(List<int> current_numbers)
        {
            return current_numbers.First();
        }
    }


    class SimpleProtocolNames
    {
        private readonly Func<string, string> GetShortTypeName = (key) => AppConstants.types_short_names[AppConstants.types_full_names.IndexOf(key)];
        private readonly Dictionary<string, int> current_period_min_numbers;
        private readonly SimpleConvert self_obj_number_converter = new();
        private readonly Dictionary<string, List<int>> type_numbers;
        
        public Dictionary<string, List<string>> Sorted_Names { get; } = [];
        public List<string>? Missed_Protocols { get; }
        public List<string>? Unknown_Protocols { get; }

        public SimpleProtocolNames(Dictionary<string, List<FileInfo>> files)
        {
            type_numbers = GetProtocolNumbers(files);

            MinimumNumbers self_obj_extreme_min = new(type_numbers);
            current_period_min_numbers = self_obj_extreme_min.Numbers;

            SimpleSort self_obj_sorter = new();

            foreach (var item in files)
            {
                Sorted_Names.Add(item.Key, self_obj_sorter.Sorting(type_numbers[item.Key], item.Value));
            }

            Missed_Protocols = ComputeMissedProtocols();
        }

        private Dictionary<string, List<int>> GetProtocolNumbers(Dictionary<string, List<FileInfo>> files)
        {
            Dictionary<string, List<int>> numbers = [];

            foreach (var item in files)
            {
                numbers.Add(item.Key, self_obj_number_converter.ConvertToNumbers(item.Value));
            }

            return numbers;
        }
                
        private List<string>? ComputeMissedProtocols()
        {
            MaximumNumbers self_obj_extreme_max = new(type_numbers);
            var max_numbers = self_obj_extreme_max.Numbers;

            List<string> missed_protocols = [];

            foreach (var item in type_numbers)
            {
                if (item.Value.Count >= 2)
                {
                    var range = Enumerable.Range(current_period_min_numbers[item.Key] + 1, max_numbers[item.Key] - current_period_min_numbers[item.Key]);

                    IEnumerable<int> missed = range.Except(item.Value);
                    List<int> missed_numbers = [.. missed];

                    foreach (int number in missed_numbers)
                    {
                        missed_protocols.Add($"{number}-{GetShortTypeName(item.Key)}");
                    }
                }
            }

            if (missed_protocols.Count > 0)
            {
                return missed_protocols;
            }
            else
            {
                return null;
            }
        }

        public List<string>? ComputeUnknownProtocols(Dictionary<string, List<FileInfo>> previous_period_files)
        {
            MaximumNumbers self_obj_previous_max = new(GetProtocolNumbers(previous_period_files));
            var numbers = self_obj_previous_max.Numbers;

            List<string> unknown_protocols = [];

            foreach (var item in current_period_min_numbers!)
            {
                int min_number = numbers[item.Key];   
                int max_number = item.Value;

                bool unknowns_ok = (min_number < max_number) && ((max_number - 1) != min_number);

                if (unknowns_ok)
                {
                    for (int start_num = min_number + 1; start_num < max_number; start_num++)
                    {
                        unknown_protocols.Add($"{start_num}-{GetShortTypeName(item.Key)}");
                    }
                }
            }

            if (unknown_protocols.Count is not 0)
            {
                return unknown_protocols;
            }
            else
            {
                return null;
            }
        }
    }


    class MonthBackupSums
    {
        public readonly SimpleProtocolNames? self_obj_names;
        public Dictionary<string, int> All_Protocols { get; } = IGeneralSums.CreateTable();
        public Dictionary<string, int>? Simple_Protocols_Sums { get; }
        
        public MonthBackupSums(List<FileInfo>? eias_files, Dictionary<string, List<FileInfo>>? simple_files, Dictionary<string, List<FileInfo>>? previous_period_simple_files = null)
        {
            if (eias_files is not null)
            {
                All_Protocols[AppConstants.others_sums[0]] += eias_files.Count;
                All_Protocols[AppConstants.others_sums[1]] = eias_files.Count;
            }

            if (simple_files is not null)
            {
                Simple_Protocols_Sums = ISimpleProtocolsSums.CreateTable();
                                                                
                foreach (var item in simple_files)
                {
                    Simple_Protocols_Sums![item.Key] = item.Value.Count;
                    All_Protocols[AppConstants.others_sums[2]] += item.Value.Count;
                }
                All_Protocols[AppConstants.others_sums[0]] += All_Protocols[AppConstants.others_sums[2]];
                CalcProtocolTypeFullSum();
                CalcProtocolLocationSums();

                self_obj_names = new(simple_files);

                if (previous_period_simple_files is not null)
                {
                    self_obj_names.ComputeUnknownProtocols(previous_period_simple_files);
                }

                CalcNotFoundProtocolsSums();
            }
        }
                
        private void CalcProtocolTypeFullSum()
        {
            for (int type_index = 0, calc_index = 0; type_index < AppConstants.full_type_sums.Count; type_index++)
            {
                Simple_Protocols_Sums![AppConstants.full_type_sums[type_index]] = Simple_Protocols_Sums[AppConstants.types_full_names[calc_index]] + Simple_Protocols_Sums[AppConstants.types_full_names[calc_index + 1]];
                calc_index += 2;
            }
        }

        private void CalcProtocolLocationSums()
        {
            for (int city_index = 0, calc_index = 0; city_index < AppConstants.full_location_sums.Count; city_index++)
            {
                Simple_Protocols_Sums![AppConstants.full_location_sums[city_index]] = Simple_Protocols_Sums[AppConstants.types_full_names[calc_index]] + Simple_Protocols_Sums[AppConstants.types_full_names[calc_index + 2]] + Simple_Protocols_Sums[AppConstants.types_full_names[calc_index + 4]];
                calc_index += 1;
            }
        }

        private void CalcNotFoundProtocolsSums()
        {
            if (self_obj_names!.Missed_Protocols is not null)
            {
                Simple_Protocols_Sums![AppConstants.not_found_sums[0]] = self_obj_names.Missed_Protocols.Count;
            }

            if (self_obj_names.Unknown_Protocols is not null)
            {
                Simple_Protocols_Sums![AppConstants.not_found_sums[1]] = self_obj_names.Unknown_Protocols.Count;
            }
        }
    }
}
