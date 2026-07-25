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
    [NativeHook("Delegate")]
    public class DelegateHook : INativeHook
    {
        public ClassNode GetClassNode()
        {
            var methods = new List<MethodNode>();

            // Constructor(target, methodName)
            var ctorParams = new List<ParameterNode> { 
                new ParameterNode("target", "object"), 
                new ParameterNode("methodName", TypeRef.String) 
            };
            
            methods.Add(new MethodNode("constructor", ctorParams, TypeRef.Void, false, 
                new NativeMethod(args => {
                    var loader = ProgramLoader.Current;
                    if (loader == null || loader.MainModule == null) return new Value<object>(null);

                    var target = args[0]?.Data;
                    var methodName = args[1]?.Data as string;

                    if (methodName == null) return new Value<object>(null);

                    // Store metadata directly on the delegate instance fields
                    // Assuming the IR class has fields for 'methodName' and 'target'
                    var self = loader.GetCurrentThis();
                    if (self != null) {
                        var fieldsProp = self.GetType().GetProperty("Fields");
                        if (fieldsProp != null) {
                            var fields = (IDictionary<string, object>)fieldsProp.GetValue(self)!;
                            fields["Target"] = target;
                            fields["MethodName"] = methodName;
                        }
                    }
                    return new Value<object>(null);
                })));

            var node = new ClassNode("Delegate", new List<string>(), 
                new List<FieldNode> { new FieldNode("Target", "object"), new FieldNode("MethodName", TypeRef.String) }, 
                new List<ConstructorNode>(), methods);
            
            node.Interfaces.Add("IDelagate");
            return node;
        }
    }
}