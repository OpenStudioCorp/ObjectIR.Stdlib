using ObjectIR.Core;
using ObjectIR.Stdlib.Core;
using ObjectIR.StdLib.Core.Core;
namespace ObjectIR.Stdlib.System
{
    public class IO<T> : Core.Computer.IO  where T : IEquatable<T>, IComparable<T>
    {
        public static TextReader Reader;
        public static Stream Out { get; private set; }
        public static Stream Error { get; private set; }
        public IO() { }
        public static IO<T> Default { get; } = new IO<T>();

        public static void Print(Value<string> data)
        {
            Console.Write(data.Data);
        }

        public static void Print<T1>(Value<T1> data)
        {
            Console.Write(data.Data);
        }

        public static void Println<T1>(Value<T1> data)
        {
            Console.WriteLine(data.Data);
        }

        public static void Print(Value<string> format, params Value<AnyValue>[] data)
        {
            Console.Write(format.Data, data.Select(d => d.Data).ToArray());
        }

        public static void Println(Value<string> format, params Value<AnyValue>[] data)
        {
            Console.WriteLine(format.Data, data.Select(d => d.Data).ToArray());
        }

        public static Value<string> ReadLine()
        {
            Value<string> value;
            using (var reader = Reader)
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
            throw new NotImplementedException();
        }

        public static void SetForegroundColor(ConsoleColor color)
        {
            throw new NotImplementedException();
        }
    }
}
