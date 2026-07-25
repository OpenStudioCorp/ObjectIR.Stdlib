using ObjectIR.Core.AST;
using System;
using System.Collections.Generic;
using System.Text;

using System.Collections.Concurrent;

namespace ObjectIR.StdLib.Core.Generics
{
    public interface IDelagate
    {
        string Name { get; }
        MethodReference Method { get; }
        object? Target { get; }
        string Id { get; }
    }

    /// <summary>
    /// Provides a shared registry to associate guest object IDs (via metadata)
    /// with their native delegate metadata.
    /// </summary>
    public static class DelegateRegistry
    {
        private static readonly ConcurrentDictionary<string, IDelagate> _storage = new();
        private static int _nextId = 0;

        public static string GetNextId() => Interlocked.Increment(ref _nextId).ToString();

        public static void Register(string id, IDelagate metadata)
        {
            _storage[id] = metadata;
           
            // Console.WriteLine($"[REGISTRY] Registered ID {id} for delegate {metadata.Name}");
        }

        public static bool TryGetMetadata(string id, out IDelagate? metadata)
        {
            var success = _storage.TryGetValue(id, out metadata);
            if (!success) Console.WriteLine($"[REGISTRY] Lookup failed for ID {id}. Registry has {_storage.Count} items.");
            return success;
        }
    }
}
