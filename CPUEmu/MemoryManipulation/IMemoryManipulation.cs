using CpuContract;
using CpuContract.Memory;

namespace CPUEmu.MemoryManipulation
{
    public interface IMemoryManipulation
    {
        int Offset { get; }

        void Execute(IMemoryMap memoryMapMap);
    }
}
