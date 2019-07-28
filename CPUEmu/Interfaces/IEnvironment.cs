using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu.Interfaces
{
    public interface IEnvironment : IDisposable
    {
        ICpuState CpuState { get; }
        IMemoryMap MemoryMap { get; }
        int PayloadAddress { get; }
        int StackAddress { get; }
        int StackSize { get; }
        IInterruptBroker InterruptBroker { get; set; }

        void Reset();
    }
}
