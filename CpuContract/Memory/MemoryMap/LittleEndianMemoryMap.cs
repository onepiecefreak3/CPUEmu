using CpuContract.Attributes;
using CpuContract.Memory.MemoryRegion;

namespace CpuContract.Memory.MemoryMap
{
    [UniqueIdentifier("LittleEndian")]
    public class LittleEndianMemoryMap : LittleEndianMemoryRegion, IMemoryMap
    {
        public IMemoryRegion Payload { get; }
        public IMemoryRegion Stack { get; }

        public LittleEndianMemoryMap(int memorySize, int payloadAddress, int payloadSize, int stackAddress, int stackSize) :
            base(new byte[memorySize], 0, memorySize)
        {
            Payload = new LittleEndianMemoryRegion(memory, payloadAddress, payloadSize);
            Stack = new LittleEndianMemoryRegion(memory, stackAddress, stackSize);
        }

        public IMemoryRegion GetRegion(int offset, int size)
        {
            return new LittleEndianMemoryRegion(memory, offset, size);
        }
    }
}
