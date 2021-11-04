using System;

namespace CpuContract.Memory
{
    public interface IMemoryMap : IMemoryRegion, IDisposable
    {
        IMemoryRegion Payload { get; }
        IMemoryRegion Stack { get; }

        IMemoryRegion GetRegion(int offset, int size);
    }
}
