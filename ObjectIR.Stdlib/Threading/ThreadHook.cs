using ObjectIR.Core;
using ObjectIR.Core.AST;
using ObjectIR.StdLib.Core.Generics;
using ObjectIR.StdLib.Core.Memory;
using System;
using System.Collections.Generic;

namespace ObjectIR.Stdlib.Threading
{
    [NativeHook("Thread")]
    public class ThreadHook : INativeHook
    {
        public ClassNode GetClassNode()
        {
            var methods = new List<MethodNode>();

            // Spawn
            methods.Add(new MethodNode("Spawn", 
                new[] { new ParameterNode("entryPoint", "IDelagate") }, 
                TypeRef.Void, true, 
                new NativeMethod(args => {
                    var loader = ProgramLoader.Current;
                    if (loader != null && args[0].Data is IDelagate del)
                    {
                        loader.SpawnThread(del);
                    }
                    return new Value<object>(null);
                })));

            // Sleep
            methods.Add(new MethodNode("Sleep", 
                new[] { new ParameterNode("ms", TypeRef.Int32) }, 
                TypeRef.Void, true, 
                new NativeMethod(args => {
                    System.Threading.Thread.Sleep(Convert.ToInt32(args[0].Data));
                    return new Value<object>(null);
                })));

            var node = new ClassNode("Thread", new List<string>(), new List<FieldNode>(), new List<ConstructorNode>(), methods);
            node.IsStatic = true;
            return node;
        }
    }
}