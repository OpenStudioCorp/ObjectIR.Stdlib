using ObjectIR.Core;
using ObjectIR.Stdlib.Core;
using ObjectIR.StdLib.Core.Core;
namespace ObjectIR.Stdlib.System
{
    public class IO : Core.Computer.IO
    {
        
        public static TextWriter Out { get; private set; } = Console.Out;
        public static TextWriter Error { get; private set; } = Console.Error;
        public static TextReader In { get; private set; } = Console.In;
        public IO() { }
  

        public static void Print(Value<Object> data)
        {
            Out.Write(data);
        }
        public static void Print(Value<Object> format, params Value<Object>[] data)
        {
            Out.Write(format.ToString(), data.Select(d => d.Data).ToArray());
        }

        public static void Println(Value<Object> format, params Value<Object>[] data)
        {
            Out.WriteLine(format.ToString(), data.Select(d => d.Data).ToArray());
        }

        public static Value<string> ReadLine()
        {
            Value<string> value;
            using (var reader = In)
            {
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                    {
                        value = new Value<string>(string.Empty);
                        break;
                    }
                    value = new Value<string>(line);
                    break;
                }
            }
            return value;
        }

        public static void SetBackgroundColor(ConsoleColor color)
        {
            Console.BackgroundColor = color; 
        }

        public static void SetForegroundColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }
    }
}
