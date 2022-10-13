using System.Diagnostics;
using System.Reflection;

namespace ReflectionForDic
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Assembly? a = null;
            try
            {
                a  = Assembly.LoadFrom("FastRead"); // Загрузить необходимую сборку
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); return;
            }
            if (a is not null)
            {
                var type = a.GetTypes().FirstOrDefault(v => v.Name == "Program");// получить класс Program
                var methods = type?.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)  // получить приватный статический метод (там будет один)
                              ?? throw new ArgumentNullException("type?.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)");
                var program = Activator.CreateInstance(type) ?? throw new ArgumentNullException("Activator.CreateInstance(type)"); // создать экземляр класса Program
                var path = @"../../../../"; // путь к файлу
                var inputFile = path + "test.txt";               
                // Параллельный вариант
                var time = new Stopwatch();
                time.Start();
                if (methods[0].Invoke(program, new[] { inputFile }) is not Dictionary<string, uint> result) throw new ArgumentNullException(nameof(result));
                time.Stop();

                // Последовательный вариант
                var time_sq = new Stopwatch();
                time_sq.Start();
                if (methods[1].Invoke(program, new[] { inputFile }) is not Dictionary<string, uint> result_sq) throw new ArgumentNullException(nameof(result_sq));
                time_sq.Stop();

                var resultFile = path + @"parallel_result.txt";
                var resultFileSq = path + $"sequential_result.txt";

                if (File.Exists(resultFile))
                    File.Delete(resultFile);
                else
                    File.Create(resultFile).Close();

                if (File.Exists(resultFileSq))
                    File.Delete(resultFileSq);
                else
                    File.Create(resultFileSq).Close();

                using (StreamWriter sw = new(resultFile))
                    foreach (var pair in result)
                        sw.WriteLine(pair);

                using (StreamWriter sw = new(resultFileSq))
                    foreach (var pair in result_sq)
                        sw.WriteLine(pair);
                Console.WriteLine("Program has completed successfully");
                Console.WriteLine($"Elapsed time for parallel: {time.ElapsedMilliseconds} ms");
                Console.WriteLine($"Elapsed time for sequential: {time_sq.ElapsedMilliseconds} ms");
                Console.WriteLine($"Acceration: {(time_sq.ElapsedMilliseconds * 1.0 / time.ElapsedMilliseconds):F4} ms");
                time.Reset();
                time_sq.Reset();
            }
        }
    }
}