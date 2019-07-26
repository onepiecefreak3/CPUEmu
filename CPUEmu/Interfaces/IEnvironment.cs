using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu.Interfaces
{
    public interface IEnvironment
    {
        ICpuState CpuState { get; }
        IMemoryMap MemoryMap { get; }
        IInterruptBroker InterruptBroker { get; }
    }
}
