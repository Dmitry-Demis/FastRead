using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FastRead
{
    internal class Program
    {
        //static void Main(string[] args)
        //{
        //    Console.OutputEncoding = System.Text.Encoding.UTF8;
        //    ParallelCheck();
        //}
        private static Dictionary<string, uint> ParallelCheck(string path)
        {
            //var path = @"../../../";
            var separators = new[] { ',', '.', ' ', ';', ':', '?', '!', '(', ')', '*', '"', '[', ']' };
            var result = new ConcurrentDictionary<string, uint>(StringComparer.InvariantCultureIgnoreCase);

            File.ReadLines(path)
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
            //var resultFile = path + @"\parallel_result.txt";
            //if (File.Exists(resultFile))
            //    File.Delete(resultFile);
            //else
            //    File.Create(resultFile).Close();

            //using (StreamWriter sw = new StreamWriter(resultFile))
            //{
            //    foreach (var pair in res) sw.WriteLine(pair);
            //}
            return res;
        }
        /*private static void SequentialCheck()
        {
            var path = @"../../../";
            var separators = new[] { ',', '.', ' ', ';', ':', '?', '!', '(', ')', '*', '"', '[', ']' };
            var sq = new Dictionary<string, uint>(StringComparer.InvariantCultureIgnoreCase);
            using (StreamReader sr = new(path + @"\test.txt"))
            {
                string? line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.ToLower();
                    line = Regex.Replace(line, @"\[\d+\]", " "); // footnotes
                    line = Regex.Replace(line, @"^m{0,4}(cm|cd|d?c{0,3})?(xc|xl|l?x{0,3})?(ix|iv|v?i{0,3})?\.$", " "); // romain letters
                    line = Regex.Replace(line, @"\-\s", " "); // double dashes
                    line = Regex.Replace(line, @"[0-9]+", " ");
                    foreach (var word in line.Split(separators, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!sq.ContainsKey(word))
                        {
                            sq.Add(word, 1);
                        }
                        else
                        {
                            sq[word]++;
                        }
                    }
                }
            }
            using (StreamWriter sw = new StreamWriter(path + @"\sequential_result.txt"))
            {
                foreach (var pair in sq.OrderByDescending(u => u.Value)) sw.WriteLine(pair);
            }
        }*/
    }
}
