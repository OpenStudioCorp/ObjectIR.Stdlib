using ObjectIR.Core;

namespace ObjectIR.Stdlib.Core.Computer
{
    public interface IO
    {
        public static TextReader In { get; }
        public static TextWriter Out { get; }
        public static TextWriter Error { get; }

        public static ConsoleColor ForegroundColor { get; set; }
        public static ConsoleColor BackgroundColor { get; set; }

        public static abstract void Print<T>(Value<T> data);
        public static abstract void Println<T>(Value<T> data);

        public static abstract void Print(Value<string> format, params Value<StdLib.Core.Core.AnyValue>[] data);
        public static abstract void Println(Value<string> format, params Value<StdLib.Core.Core.AnyValue>[] data);

        public static abstract Value<string> ReadLine();

        public static abstract void SetBackgroundColor(ConsoleColor color);
        public static abstract void SetForegroundColor(ConsoleColor color);
    }
}
