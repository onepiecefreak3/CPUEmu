using CpuContract.Attributes;
using CpuContract.Memory.MemoryRegion;

namespace CpuContract.Memory.MemoryMap
{
    [UniqueIdentifier("BigEndian")]
    public class BigEndianMemoryMap : BigEndianMemoryRegion, IMemoryMap
    {
        public IMemoryRegion Payload { get; }
        public IMemoryRegion Stack { get; }

        public BigEndianMemoryMap(int memorySize, int payloadAddress, int payloadSize, int stackAddress, int stackSize) :
            base(new byte[memorySize], 0, memorySize)
        {
            Payload = new BigEndianMemoryRegion(memory, payloadAddress, payloadSize);
            Stack = new BigEndianMemoryRegion(memory, stackAddress, stackSize);
        }

        public IMemoryRegion GetRegion(int offset, int size)
        {
            return new BigEndianMemoryRegion(memory, offset, size);
        }
    }
}
