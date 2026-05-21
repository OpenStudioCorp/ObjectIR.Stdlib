using ObjectIR.Core;
using ObjectIR.Core.Ast;
using ObjectIR.Core.AST;
using ObjectIR.StdLib.Core.Generics;
using ObjectIR.StdLib.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ObjectIR.Stdlib.Generics
{
    [NativeHook("Func")]
    public class FuncHook : INativeHook
    {
        private static readonly ConditionalWeakTable<object, string> _delegateIdMap = new();

        public ClassNode GetClassNode()
        {
            var methods = new List<MethodNode>();

            var ctorParams = new List<ParameterNode> { 
                new ParameterNode("instance", "object"), 
                new ParameterNode("methodName", TypeRef.String) 
            };
            
            methods.Add(new MethodNode("constructor", ctorParams, TypeRef.Void, false, 
                new NativeMethod(args => {
                    var loader = ProgramLoader.Current;
                    if (loader == null || loader.MainModule == null) return new Value<object>(null);

                    var methodTarget = args[0]?.Data;
                    var methodName = args[1]?.Data as string;

                    if (methodName == null) return new Value<object>(null);

                    foreach (var cls in loader.MainModule.Classes)
                    {
                        var method = cls.Methods.FirstOrDefault(m => m.Name == methodName);
                        if (method != null)
                        {
                            var methRef = new MethodReference(
                                new TypeRef(cls.Name), 
                                method.Name, 
                                method.ReturnType, 
                                method.Parameters.Select(p => p.ParameterType).ToList());
                            
                            var self = loader.GetCurrentThis();
                            if (self != null)
                            {
                                var del = new Delagate(methRef, methodTarget);
                                DelegateRegistry.Register(del.Id, del);
                                _delegateIdMap.Add(self, del.Id);
                            }
                            break;
                        }
                    }
                    return new Value<object>(null);
                })));
// Invoke() -> object
methods.Add(new MethodNode("Invoke", new List<ParameterNode>(), "object", false, 
    new NativeMethod(args => {
        var loader = ProgramLoader.Current;
        if (loader == null) return new Value<object>(null);

        var self = loader.GetCurrentThis();
        string? delegateId = null;
        if (self != null)
        {
            var prop = self.GetType().GetProperty("Metadata");
            if (prop != null)
            {
                var metadata = (IDictionary<string, object>)prop.GetValue(self)!;
                metadata.TryGetValue("DelegateId", out var idObj);
                delegateId = idObj as string;
            }
        }

        if (delegateId != null && DelegateRegistry.TryGetMetadata(delegateId, out var del))
        {
            var invokeArgs = args?.Select(a => a?.Data).ToArray() ?? global::System.Array.Empty<object>();
            return loader.ExecuteMethod(del!.Method, del.Target, invokeArgs!);
        }
        return new Value<object>(null);
    })));


            var node = new ClassNode("Func", new List<string>(), new List<FieldNode>(), new List<ConstructorNode>(), methods);
            node.Interfaces.Add("IDelagate");
            return node;
        }
    }
}