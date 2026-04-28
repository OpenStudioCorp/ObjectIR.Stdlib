using ObjectIR.Core.AST;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectIR.StdLib.Core.Generics
{
    public interface IDelagate
    {
        string Name { get; }
        MethodReference Method { get; }

    }
}
