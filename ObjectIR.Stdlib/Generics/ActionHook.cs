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
    [NativeHook("Action")]
    public class ActionHook : INativeHook
    {
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
                                string id = Guid.NewGuid().ToString();
                                DelegateRegistry.Register(id, del);
                                
                                // Console.WriteLine($"[ACTIONHOOK] self type: {self.GetType().FullName}");
                                // Lattice ManagedObject uses 'Fields' dictionary
                                var fieldsProp = self.GetType().GetProperty("Fields");
                                if (fieldsProp != null)
                                {
                                    var fields = (IDictionary<string, object>)fieldsProp.GetValue(self)!;
                                    fields["DelegateId"] = id;
                                    // Console.WriteLine($"[ACTIONHOOK] Registered delegate {del.Name} with ID {id} in Fields");
                                }
                                else 
                                {
                                    Console.WriteLine($"[ACTIONHOOK] Could NOT find 'Fields' property on {self.GetType().FullName}");
                                    // Try to list all properties
                                    foreach (var p in self.GetType().GetProperties()) {
                                        Console.WriteLine($"[ACTIONHOOK]   Available property: {p.Name}");
                                    }
                                }
                            }
                            else 
                            {
                                Console.WriteLine("[ACTIONHOOK] loader.GetCurrentThis() returned null");
                            }
                            break;
                        }
                    }
                    return new Value<object>(null);
                })));

            methods.Add(new MethodNode("Invoke", new List<ParameterNode>(), TypeRef.Void, false, 
                new NativeMethod(args => {
                    var loader = ProgramLoader.Current;
                    if (loader == null) return new Value<object>(null);

                    var self = loader.GetCurrentThis();
                    string? delegateId = null;
                    if (self != null)
                    {
                        var fieldsProp = self.GetType().GetProperty("Fields");
                        if (fieldsProp != null)
                        {
                            var fields = (IDictionary<string, object>)fieldsProp.GetValue(self)!;
                            fields.TryGetValue("DelegateId", out var idObj);
                            delegateId = idObj as string;
                        }
                    }

                    if (delegateId != null && DelegateRegistry.TryGetMetadata(delegateId, out var del))
                    {
                        var invokeArgs = args?.Select(a => a?.Data).ToArray() ?? global::System.Array.Empty<object>();
                        loader.ExecuteMethod(del!.Method, del.Target, invokeArgs!);
                    }
                    return new Value<object>(null);
                })));

            var node = new ClassNode("Action", new List<string>(), new List<FieldNode>(), new List<ConstructorNode>(), methods);
            node.Interfaces.Add("IDelagate");
            return node;
        }
    }
}