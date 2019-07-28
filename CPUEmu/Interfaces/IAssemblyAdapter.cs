using System;
using System.Collections.Generic;
using System.IO;

namespace CPUEmu.Interfaces
{
    public interface IAssemblyAdapter : IDisposable
    {
        IList<IInstruction> Instructions { get; }
        IEnvironment Environment { get; }
        Executor Executor { get; }

        bool Identify(Stream file);

        void Load(Stream file);
    }
}
