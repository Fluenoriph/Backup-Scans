using System;
using System.Numerics;
using System.Text.RegularExpressions;
using TextData;
using Tracing;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace BackupBlock
{
    struct CurrentYear
    {
        public static string Year
        {
            get
            {
                DateTime current_date = DateTime.Now;
                return current_date.Year.ToString();
            }
        }
    }


    class SourceFiles
    {
        private readonly FileInfo[] files;
        public string File_Type { get; set; } = AppConstants.scan_file_type;

        public SourceFiles(string drive_directory)
        {
            DirectoryInfo directory = new(drive_directory);
            files = directory.GetFiles($"*.{File_Type}");
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


    class SimpleProtocolsNumbers
    {
        public Dictionary<string, List<int>> Numbers { get; } = [];
        public Dictionary<string, List<string>> Sorted_Names { get; } = [];

        public SimpleProtocolsNumbers(Dictionary<string, List<FileInfo>> files)
        {
            SimpleConvert self_obj_number_converter = new();
            SimpleSort self_obj_sorter = new();

            foreach (var item in files)
            {
                var numbers = self_obj_number_converter.ConvertToNumbers(item.Value);
                
                Numbers.Add(item.Key, numbers);
                Sorted_Names.Add(item.Key, self_obj_sorter.Sorting(numbers, item.Value));
            }
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
}

