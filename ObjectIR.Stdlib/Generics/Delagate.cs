using ObjectIR.Core.AST;
using ObjectIR.StdLib.Core.Generics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectIR.Stdlib.Generics
{
    public class Delagate : IDelagate
    {
        public string Name { get; }
        public MethodReference Method { get; }
        public object? Target { get; }
        public string Id { get; }

        public Delagate(MethodReference meth, object? target = null) {
            this.Method = meth;
            this.Name = meth.Name;
            this.Target = target;
            this.Id = Guid.NewGuid().ToString(); // Unique ID for this specific delegate
            // Console.WriteLine($"[DELEGATE] Created delegate {this.Name} with ID {this.Id}");
        }
    }
}
