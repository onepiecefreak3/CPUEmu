using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu.Interfaces
{
    public interface IArchitectureExecuter
    {
        ICpuState CpuState { get; }
        IList<IInstruction> Instructions { get; }

        void ExecuteNextInstruction();
    }
}
