using System;

namespace CpuContract.Memory
{
    public abstract class BaseMemoryMapMap : IMemoryMap
    {
        private byte[] _memoryMap;

        public MemoryRegion Payload { get; }
        public MemoryRegion Stack { get; }
        public int Length => _memoryMap.Length;

        public BaseMemoryMapMap(int memorySize, MemoryRegion payload, MemoryRegion stack)
        {
            _memoryMap = new byte[memorySize];

            Payload = payload;
            Stack = stack;
        }

        public byte ReadByte(int offset)
        {
            var buffer = new byte[1];
            Read(buffer, 0, 1, offset);
            return buffer[0];
        }

        public void WriteByte(int offset, byte value)
        {
            Write(new[] { value }, 0, 1, offset);
        }

        public abstract ushort ReadUInt16(int offset);
        public abstract void WriteUInt16(int offset, ushort value);

        public abstract uint ReadUInt32(int offset);
        public abstract void WriteUInt32(int offset, uint value);

        public void Read(byte[] buffer, int offset, int count, int offsetInMemory)
        {
            Array.Copy(_memoryMap, offsetInMemory, buffer, offset, count);
        }

        public void Write(byte[] buffer, int offset, int count, int offsetInMemory)
        {
            Array.Copy(buffer, offset, _memoryMap, offsetInMemory, count);
        }

        public void ClearAll()
        {
            Clear(0, _memoryMap.Length);
        }

        public void Clear(int offsetInMemory, int count)
        {
            Array.Clear(_memoryMap, offsetInMemory, count);
        }

        public void Dispose()
        {
            Array.Clear(_memoryMap, 0, _memoryMap.Length);
            _memoryMap = null;
        }
    }
}
