using ObjectIR.Core;
using ObjectIR.Core.Ast;
using ObjectIR.Core.AST;
using ObjectIR.StdLib.Core.Generics;
using ObjectIR.StdLib.Core.Memory;
using ObjectIR.Stdlib.Generics;
using System;
using System.Collections.Generic;
using System.Linq;

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
                new[] { new ParameterNode("delegate", "object") }, 
                TypeRef.Void, true, 
                new NativeMethod(args => {
                    var loader = ProgramLoader.Current;
                    if (loader != null && loader.MainModule != null)
                    {
                        var delObj = args[0].Data;
                        if (delObj != null)
                        {
                            IDelagate? del = null;

                            // 1. Try to get DelegateId from Fields
                            var fieldsProp = delObj.GetType().GetProperty("Fields");
                            if (fieldsProp != null)
                            {
                                var fields = (IDictionary<string, object>)fieldsProp.GetValue(delObj)!;
                                if (fields.TryGetValue("DelegateId", out var idObj) && idObj is string id)
                                {
                                    DelegateRegistry.TryGetMetadata(id, out del);
                                }
                                else if (fields.TryGetValue("MethodName", out var methNameObj) && methNameObj is string methodName)
                                {
                                    // Fallback for objects that store MethodName and Target directly in fields
                                    fields.TryGetValue("Target", out var target);
                                    
                                    // Resolve the method
                                    foreach (var cls in loader.MainModule.Classes)
                                    {
                                        var method = cls.Methods.FirstOrDefault(m => m.Name == methodName);
                                        if (method != null)
                                        {
                                            var methRef = new MethodReference(new TypeRef(cls.Name), method.Name, method.ReturnType, method.Parameters.Select(p => p.ParameterType).ToList());
                                            del = new Delagate(methRef, target);
                                            break;
                                        }
                                    }
                                }
                            }

                            if (del != null)
                            {
                                loader.SpawnThread(del);
                            }
                            else 
                            {
                                Console.WriteLine("[THREADHOOK] Could not resolve delegate for Spawn");
                            }
                        }
                    }
                    return new Value<object>(null);
                })));

            // Sleep
            methods.Add(new MethodNode("Sleep", 
                new[] { new ParameterNode("ms", TypeRef.Int32) }, 
                TypeRef.Void, true, 
                new NativeMethod(args => {
                    var loader = ProgramLoader.Current;
                    if (loader != null)
                    {
                        loader.Yield(Convert.ToInt32(args[0].Data));
                    }
                    return new Value<object>(null);
                })));

            var node = new ClassNode("Thread", new List<string>(), new List<FieldNode>(), new List<ConstructorNode>(), methods);
            node.IsStatic = true;
            return node;
        }
    }
}