using ObjectIR.Core;
using ObjectIR.Core.AST;
using ObjectIR.StdLib.Core.Generics;
using ObjectIR.StdLib.Core.Memory;
using ObjectIR.StdLib.Core.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectIR.Stdlib.Threading
{
    public class Thread : IThread
    {
        public static void Spawn(IDelagate entryPoint)
        {
            var loader = ProgramLoader.Current;
            if (loader == null) return;

            // This needs to interact with the runtime's scheduler.
            // Since ProgramLoader doesn't have a Spawn method yet, 
            // we might need to cast or add it to the interface.
            
            if (loader is IThreadManager threadManager)
            {
                threadManager.SpawnThread(entryPoint);
            }
        }

        public static void Yield()
        {
            // Implementation depends on runtime scheduler
        }

        public static void Sleep(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }
    }

    /// <summary>
    /// Extended interface for the runtime to support thread management.
    /// </summary>
    public interface IThreadManager
    {
        void SpawnThread(IDelagate entryPoint);
    }
}