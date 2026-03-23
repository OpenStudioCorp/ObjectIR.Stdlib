using ObjectIR.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectIR.Stdlib.Math
{
    public interface Numbers
    {
        public static Value<int> add(Value<int> a, Value<int> b ) => new Value<int>(a.Data + b.Data);
    }
}
