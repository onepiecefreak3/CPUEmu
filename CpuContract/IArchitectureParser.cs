using System;
using System.Collections.Generic;
using System.IO;

namespace CpuContract
{
    public interface IArchitectureParser : IDisposable
    {
        IList<IInstruction> ParseAssembly(Stream file, int baseAddress);
    }
}
