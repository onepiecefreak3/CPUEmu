using CPUEmu.Defaults;
using CPUEmu.Interfaces;

namespace CPUEmu.Aarch32
{
    class Aarch32Environment : IEnvironment
    {
        public ICpuState CpuState { get; private set; }
        public IMemoryMap MemoryMap { get; private set; }
        public int PayloadAddress { get; private set; }
        public int StackAddress { get; private set; }
        public int StackSize { get; private set; }
        public IInterruptBroker InterruptBroker { get; set; }

        public Aarch32Environment(int memorySizeInBytes, int payloadAddress, int stackAddress, int stackSize)
        {
            ResetInternal(memorySizeInBytes, payloadAddress, stackAddress, stackSize);
        }

        public void Reset()
        {
            ResetInternal(MemoryMap.Length, PayloadAddress, StackAddress, StackSize);
        }

        private void ResetInternal(int memorySizeInBytes, int payloadAddress, int stackAddress, int stackSize)
        {
            CpuState?.Dispose();

            CpuState = new Aarch32CpuState((uint)stackAddress);
            if (MemoryMap == null)
                MemoryMap = new LittleEndianMemoryMap(memorySizeInBytes);
            else
                MemoryMap.Clear();
            PayloadAddress = payloadAddress;
            StackAddress = stackAddress;
            StackSize = stackSize;
        }

        public void Dispose()
        {
            CpuState?.Dispose();
            MemoryMap?.Dispose();
            InterruptBroker?.Dispose();

            CpuState = null;
            MemoryMap = null;
            InterruptBroker = null;
        }
    }
}
