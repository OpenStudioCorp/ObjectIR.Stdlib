using ObjectIR.Core.AST;
using ObjectIR.StdLib.Core.Generics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectIR.Stdlib.Generics
{
    internal class Delagate : IDelagate
    {
        public string Name { get;}
        public MethodReference Method { get; }
        public Delagate(MethodReference meth) {
            this.Method = meth;
            this.Name = meth.Name;
        }

    }
}
