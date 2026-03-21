using ObjectIR.Core;
using ObjectIR.Stdlib.Core;
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
            throw new NotImplementedException();
        }
    }
}
