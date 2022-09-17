using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace FastRead
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var path = @"../../";
            var separators = new[] { ',', '.', ' ', ';', ':', '?', '!', '(', ')', '*', '"', '[', ']' };
            var result = new ConcurrentDictionary<string, uint>(StringComparer.InvariantCultureIgnoreCase);
            File.ReadLines(path + @"\test.txt")
                .AsParallel()
                .ForAll(line =>
                {
                    line = line.ToLower();
                    line = Regex.Replace(line, @"\[\d+\]", " "); // footnotes
                    line = Regex.Replace(line, @"^m{0,4}(cm|cd|d?c{0,3})?(xc|xl|l?x{0,3})?(ix|iv|v?i{0,3})?\.$", " "); // romain letters
                    line = Regex.Replace(line, @"\-\s", " "); // double dashes
                    line = Regex.Replace(line, @"[0-9]+", " ");
                    foreach (var word in line.Split(separators, StringSplitOptions.RemoveEmptyEntries))
                    {
                        result.AddOrUpdate(word, 1, ((s, u) => u + 1));
                    }
                });
            var res = result
                .OrderByDescending(u => u.Value)
                .ToDictionary(d => d.Key, d => d.Value);
            var resultFile = path + @"\result.txt";
            if (File.Exists(resultFile))
                File.Delete(resultFile);
            else
                File.Create(resultFile).Close();

            using (StreamWriter sw = new StreamWriter(resultFile))
            {
                foreach (var pair in res) sw.WriteLine(pair);
            }
        }
    }

}