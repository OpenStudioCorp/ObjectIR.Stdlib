using System;
using System.Collections.Generic;
using System.Text;
using ObjectIR.Core;
using ObjectIR.Core.AST;
namespace ObjectIR.StdLib.Core.Memory
{
    /// <summary>
    /// Defines a contract for reflection-related operations or behaviors.
    /// </summary>
    /// <remarks>Implementations of this interface typically provide mechanisms for inspecting or interacting
    /// with metadata, such as types, properties, or methods, at runtime. The specific capabilities depend on the
    /// implementing type.</remarks>
    public interface Reflection
    {
        public static abstract Value<TypeRef> GetType();
        public static abstract Value<TypeRef> Typeof(Statement T);
    }
}
