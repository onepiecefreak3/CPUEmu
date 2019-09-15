using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CpuContract.Memory;

namespace CPUEmu.MemoryManipulation
{
    public interface IMemoryManipulation
    {
        int Offset { get; }

        void Execute(BaseMemoryMap memoryMap);
    }
}
