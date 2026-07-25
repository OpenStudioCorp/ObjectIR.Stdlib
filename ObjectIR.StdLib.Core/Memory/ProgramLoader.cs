using ObjectIR.Core;
using ObjectIR.Core.AST;
using ObjectIR.StdLib.Core.Generics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace ObjectIR.StdLib.Core.Memory
{
    /// <summary>
    /// Marks a class as a provider of native implementations for a specific ObjectIR class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NativeHookAttribute : Attribute
    {
        public string TargetClassName { get; }
        public NativeHookAttribute(string targetClassName) => TargetClassName = targetClassName;
    }

    /// <summary>
    /// Interface for a native hook provider.
    /// </summary>
    public interface INativeHook
    {
        /// <summary>
        /// Returns the ClassNode containing the native method implementations.
        /// </summary>
        ClassNode GetClassNode();
    }

    /// <summary>
    /// A runtime-agnostic registry for discovering and resolving native hooks.
    /// </summary>
    public static class NativeRegistry
    {
        private static readonly ConcurrentDictionary<string, Type> _hooks = new();
        private static readonly object _astLock = new();

        /// <summary>
        /// Scans an assembly for classes marked with [NativeHook] and registers them.
        /// </summary>
        public static void RegisterFromAssembly(Assembly assembly)
        {
            var hookTypes = assembly.GetTypes()
                .Where(t => typeof(INativeHook).IsAssignableFrom(t) && t.GetCustomAttribute<NativeHookAttribute>() != null);

            foreach (var type in hookTypes)
            {
                var attr = type.GetCustomAttribute<NativeHookAttribute>();
                if (attr != null)
                {
                    _hooks[attr.TargetClassName] = type;
                }
            }
        }

        /// <summary>
        /// Attempts to resolve a hook for the given class name and register it into the program AST.
        /// Thread-safe: uses a lock to prevent duplicate registration when called concurrently.
        /// </summary>
        public static bool TryRegister(string className, ModuleNode program)
        {
            lock (_astLock)
            {
                if (program.Classes.Any(c => c.Name == className)) return true;

                if (_hooks.TryGetValue(className, out var hookType))
                {
                    var provider = (INativeHook)Activator.CreateInstance(hookType)!;
                    program.Classes.Add(provider.GetClassNode());
                    return true;
                }

                return false;
            }
        }
    }

    /// <summary>
    /// Defines a contract for accessing the main program's metadata and execution state.
    /// This interface is typically implemented by the runtime to provide the standard library
    /// with information about the currently executing ObjectIR program.
    /// </summary>
    public interface IProgramLoader
    {
        /// <summary>
        /// Gets the main module of the currently executing program.
        /// </summary>
        ModuleNode? MainModule { get; }

        /// <summary>
        /// Gets the method currently being executed in the guest environment.
        /// </summary>
        MethodNode? GetCurrentMethod();

        /// <summary>
        /// Resolves a type reference to its definition within the loaded program context.
        /// </summary>
        ClassNode? ResolveType(TypeRef typeRef);

        /// <summary>
        /// Gets the 'this' pointer of the currently executing guest method.
        /// Useful for native implementations of instance methods.
        /// </summary>
        object? GetCurrentThis();

        /// <summary>
        /// Executes an ObjectIR method from a native context.
        /// </summary>
        Value<object> ExecuteMethod(MethodReference method, object? thisObj, params object[] args);

        /// <summary>
        /// Spawns a new thread of execution.
        /// </summary>
        void SpawnThread(IDelagate entryPoint);

        /// <summary>
        /// Suspends the current guest execution context for a specified duration.
        /// </summary>
        void Yield(int milliseconds);
    }

    /// <summary>
    /// Static registry that holds the active <see cref="IProgramLoader"/> instance.
    /// Uses <see cref="AsyncLocal{T}"/> to ensure that each thread or execution context
    /// (e.g., separate CPU threads) has its own isolated loader.
    /// </summary>
    public static class ProgramLoader
    {
        private static readonly AsyncLocal<IProgramLoader?> _current = new AsyncLocal<IProgramLoader?>();

        /// <summary>
        /// Gets or sets the current program loader implementation for the current execution context.
        /// </summary>
        public static IProgramLoader? Current 
        { 
            get => _current.Value; 
            set => _current.Value = value; 
        }

        /// <summary>
        /// Temporarily activates a specific loader for the current thread/task.
        /// Usage: using (ProgramLoader.Activate(myCpuLoader)) { ... }
        /// </summary>
        public static IDisposable Activate(IProgramLoader loader)
        {
            var old = Current;
            Current = loader;
            return new Scope(() => Current = old);
        }

        private class Scope(Action onDispose) : IDisposable 
        { 
            public void Dispose() => onDispose(); 
        }
    }
}