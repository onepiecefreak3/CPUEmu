using System;
using System.Collections.Generic;
using System.IO;

namespace CPUEmu.Interfaces
{
    public interface IArchitectureParser
    {
        IList<IInstruction> Parse(Stream assembly);
    }
}
