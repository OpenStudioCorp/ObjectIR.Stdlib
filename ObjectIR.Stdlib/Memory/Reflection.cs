using ObjectIR.Core;
using ObjectIR.Core.AST;
using ObjectIR.StdLib.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ObjectIR.Stdlib.Memory
{
    public class Reflection : StdLib.Core.Memory.Reflection
    {
        public static Value<TypeRef> GetType()
        {
            StackTrace stackTrace = new StackTrace();
            // Index 1 is the method that called MyMethod
            var callerMethod = stackTrace.GetFrame(1).GetMethod();
            return new Value<TypeRef>(new TypeRef(callerMethod.GetType().Name));
        }

        public static Value<TypeRef> Typeof(Statement T)
        {
            
            return new Value<TypeRef>(new TypeRef(T.ToString())); // TODO: FIx this byllshit
        }
    }
}
