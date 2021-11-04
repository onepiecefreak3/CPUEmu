using System;

namespace CpuContract.Memory.MemoryRegion
{
    // TODO: When switching to .NET 5.0, try using Memory<T>
    public abstract class BaseMemoryRegion : IMemoryRegion
    {
        private readonly byte[] _bufferShort = new byte[2];
        private readonly byte[] _bufferInt = new byte[4];

        internal byte[] memory;

        private readonly int _offset;
        private readonly int _size;

        public int Address => _offset;
        public int Length => _size;

        public byte this[int index]
        {
            get => ReadByte(index);
            set => WriteByte(index, value);
        }

        public BaseMemoryRegion(byte[] memory, int address, int length)
        {
            this.memory = memory;

            _offset = address;
            _size = length;
        }

        public byte ReadByte(int offset)
        {
            if (offset >= _size)
                return 0;

            return memory[_offset + offset];
        }

        public void WriteByte(int offset, byte value)
        {
            if (offset >= _size)
                return;

            memory[_offset + offset] = value;
        }

        public ushort ReadUInt16(int offset)
        {
            Read(_bufferShort, 0, 2, offset);
            return ReadUInt16(_bufferShort);
        }

        protected abstract ushort ReadUInt16(byte[] buffer);

        public void WriteUInt16(int offset, ushort value)
        {
            WriteUInt16(_bufferShort, value);
            Write(_bufferShort, 0, 2, offset);
        }

        protected abstract void WriteUInt16(byte[] buffer, ushort value);

        public uint ReadUInt32(int offset)
        {
            Read(_bufferInt, 0, 4, offset);
            return ReadUInt32(_bufferInt);
        }

        protected abstract uint ReadUInt32(byte[] buffer);

        public void WriteUInt32(int offset, uint value)
        {
            WriteUInt32(_bufferInt, value);
            Write(_bufferInt, 0, 4, offset);
        }

        protected abstract void WriteUInt32(byte[] buffer, uint value);

        public void Read(byte[] buffer, int offset, int count, int offsetInMemory)
        {
            var capOffset = Math.Max(0, Math.Min(offsetInMemory, _size));
            var capCount = Math.Min(_size - capOffset, count);

            Array.Copy(memory, _offset + capOffset, buffer, offset, capCount);
        }

        public void Write(byte[] buffer, int offset, int count, int offsetInMemory)
        {
            var capOffset = Math.Max(0, Math.Min(offsetInMemory, _size));
            var capCount = Math.Min(_size - capOffset, count);

            Array.Copy(buffer, offset, memory, _offset + capOffset, capCount);
        }

        public void ClearAll()
        {
            Array.Clear(memory, _offset, _size);
        }

        public void Clear(int offsetInMemory, int count)
        {
            var capOffset = Math.Max(0, Math.Min(offsetInMemory, _size));
            var capCount = Math.Min(_size - capOffset, count);

            Array.Clear(memory, _offset + capOffset, capCount);
        }

        public void Dispose()
        {
            Array.Clear(memory, _offset, _size);
            memory = null;
        }
    }
}
