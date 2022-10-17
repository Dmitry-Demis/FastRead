using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WordFrequency
{
    public static class Frequency
    {
        public static Dictionary<string, uint> ParallelCheck(string[] elements)
        {
            var separators = new[] { ',', '.', ' ', ';', ':', '?', '!', '(', ')', '*', '"', '[', ']' };
            var result = new ConcurrentDictionary<string, uint>(StringComparer.InvariantCultureIgnoreCase);
            Parallel.ForEach(elements, line =>
            {
                line = line.ToLower();
                line = Regex.Replace(line, @"\[\d+\]", " "); // footnotes
                line = Regex.Replace(line, @"^m{0,4}(cm|cd|d?c{0,3})?(xc|xl|l?x{0,3})?(ix|iv|v?i{0,3})?\.$",
                    " "); // romain letters
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
            return res;
        }
        private static Dictionary<string, uint> SequentialCheck(string path)
        {
            var separators = new[] { ',', '.', ' ', ';', ':', '?', '!', '(', ')', '*', '"', '[', ']' };
            var sq = new Dictionary<string, uint>(StringComparer.InvariantCultureIgnoreCase);
            using (StreamReader sr = new(path))
            {
                var line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.ToLower();
                    line = Regex.Replace(line, @"\[\d+\]", " "); // footnotes
                    line = Regex.Replace(line, @"^m{0,4}(cm|cd|d?c{0,3})?(xc|xl|l?x{0,3})?(ix|iv|v?i{0,3})?\.$", " "); // romain letters
                    line = Regex.Replace(line, @"\-\s", " "); // double dashes
                    line = Regex.Replace(line, @"[0-9]+", " ");
                    foreach (var word in line.Split(separators, StringSplitOptions.RemoveEmptyEntries))
                        if (!sq.ContainsKey(word))
                            sq.Add(word, 1);
                        else
                            sq[word]++;
                }
            }
            var res = sq
                .OrderByDescending(u => u.Value)
                .ToDictionary(d => d.Key, d => d.Value);
            return res;
        }
    }
}
