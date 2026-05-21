using ObjectIR.Core;
using ObjectIR.Core.Ast;
using ObjectIR.Core.AST;
using ObjectIR.StdLib.Core.Memory;
using System;
using System.Collections.Generic;

namespace ObjectIR.Stdlib.System
{
    [NativeHook("IO")]
    public class IOHook : INativeHook
    {
        public ClassNode GetClassNode()
        {
            var methods = new List<MethodNode>();

            // Print
            methods.Add(new MethodNode("Print", 
                new[] { new ParameterNode("value", "object") }, 
                TypeRef.Void, true, 
                new NativeMethod(args => {
                    global::System.Console.Write(args[0].Data);
                    return new Value<object>(null);
                })));

            // Println
            methods.Add(new MethodNode("Println", 
                new[] { new ParameterNode("value", "object") }, 
                TypeRef.Void, true, 
                new NativeMethod(args => {
                    global::System.Console.WriteLine(args.Length > 0 ? args[0].Data : "");
                    return new Value<object>(null);
                })));

            // Readln
            methods.Add(new MethodNode("Readln", 
                new List<ParameterNode>(), 
                TypeRef.String, true, 
                new NativeMethod(args => new Value<object>(global::System.Console.ReadLine()))));

            var node = new ClassNode("IO", new List<string>(), new List<FieldNode>(), new List<ConstructorNode>(), methods);
            node.IsStatic = true;
            return node;
        }
    }
}