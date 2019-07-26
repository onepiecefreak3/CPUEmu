using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu.Interfaces
{
    public interface IInstruction
    {
        int Position { get; }

        void Execute(ICpuState cpuState);
    }
}
