using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestClient.RemoteService;

namespace TestClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SendDataClient data = new SendDataClient(); // Client creating
            var path = @"../../../";
            ConcurrentStack<string> stack = new();              //
            File.ReadLines(path + "test.txt", Encoding.UTF8)    //
                .AsParallel()                                   //
                .ForAll(line =>                                 //
                {                                               // Getting strings from a file parallelly and putting into
                    line = line.Trim();                         // a concurrent stack
                    if (line != "") stack.Push(line);           //
                });                                             //
            ConcurrentDictionary<string, uint> result = new();
            const int step = 400;
            string[] elements = new string[Math.Min(step, stack.Count)]; // We send a pack of data in amount of a step
            while (!stack.IsEmpty)
            {
                var min = Math.Min(step, stack.Count);
                if (min < elements.Length) elements = new string[min];
                stack.TryPopRange(elements, 0, min);
                var dict = data.ReceiveFile(elements); // Call the server
                Parallel.ForEach(dict, word =>
                {
                    result.AddOrUpdate(word.Key, word.Value, ((s, u) => u + word.Value)); // Get a partition dictionary from the server
                });
            }
            var res = result.OrderBy(u=> u.Key)
                .OrderByDescending(u => u.Value)
                .ToDictionary(d => d.Key, d => d.Value); //Sorting data by the key firstly, then resort by the value descending
            var resultFile = path + @"wcf_result.txt";
            if (File.Exists(resultFile))
                File.Delete(resultFile);
            else
                File.Create(resultFile).Close();
            using (StreamWriter sw = new(resultFile))
                foreach (var pair in res)
                    sw.WriteLine(pair);
            Console.WriteLine($"File \"{Path.GetFullPath(resultFile)}\" has been saved");
        }
    }
}
