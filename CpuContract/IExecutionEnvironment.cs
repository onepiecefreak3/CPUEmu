using System;
using CpuContract.Memory;

namespace CpuContract
{
    public interface IExecutionEnvironment : IDisposable
    {
        ICpuState CpuState { get; }
        BaseMemoryMap MemoryMap { get; }
        IInterruptBroker InterruptBroker { get; }

        void Reset();
    }
}
