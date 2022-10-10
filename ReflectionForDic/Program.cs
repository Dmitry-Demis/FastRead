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
                Console.WriteLine(e.Message);
                return;
            }
            if (a is not null)
            {
                var type = a.GetTypes().FirstOrDefault(v => v.Name == "Program");// получить класс Program
                var methods = type?.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)  // получить приватный статический метод (там будет один)
                              ?? throw new ArgumentNullException("type?.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)");
                var program = Activator.CreateInstance(type) ?? throw new ArgumentNullException("Activator.CreateInstance(type)"); // создать экземляр класса Program
                var path = @"../../../../"; // путь к файлу
                var result =
                    methods[0].Invoke(program, new[] { path + @"test.Txt" }) as Dictionary<string, uint>; // вызвать метод и передать ему на вход строку
                if (result == null) throw new ArgumentNullException(nameof(result));
                var resultFile = path + @"parallel_result.txt";
                if (File.Exists(resultFile))
                    File.Delete(resultFile);
                else
                    File.Create(resultFile).Close();
                using (StreamWriter sw = new StreamWriter(resultFile))
                    foreach (var pair in result)
                        sw.WriteLine(pair);
                Console.WriteLine("Program has completed successfully");
            }
        }
    }
}