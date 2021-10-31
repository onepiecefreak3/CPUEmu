using System;
using CpuContract.Memory;

namespace CpuContract
{
    public class DeviceEnvironment : IDisposable
    {
        public IMemoryMap MemoryMap { get; private set; }
        public IInterruptBroker InterruptBroker { get; private set; }

        public DeviceEnvironment(IMemoryMap memoryMap, IInterruptBroker interruptBroker = null)
        {
            MemoryMap = memoryMap;
            InterruptBroker = interruptBroker;
        }

        public void Reset()
        {
            MemoryMap?.ClearAll();
        }

        public void Dispose()
        {
            MemoryMap?.Dispose();
            InterruptBroker?.Dispose();

            MemoryMap = null;
            InterruptBroker = null;
        }
    }
}
