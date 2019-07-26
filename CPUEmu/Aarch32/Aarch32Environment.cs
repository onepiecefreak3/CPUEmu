using CPUEmu.Interfaces;

namespace CPUEmu.Aarch32
{
    class Aarch32Environment : IEnvironment
    {
        public ICpuState CpuState { get; }
        public IMemoryMap MemoryMap { get; }
        public IInterruptBroker InterruptBroker { get; }

        public Aarch32Environment(int memorySizeInBytes, IInterruptBroker broker)
        {
            CpuState = new Aarch32CpuState();
            MemoryMap = new LittleEndianMemoryMap(memorySizeInBytes);
            InterruptBroker = broker;
        }
    }
}
