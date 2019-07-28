using System;
using CPUEmu.Interfaces;

namespace CPUEmu.Defaults
{
    abstract class DefaultMemoryMap : IMemoryMap
    {
        protected byte[] Memory { get; private set; }

        public int Length { get; private set; }

        protected DefaultMemoryMap(int memorySizeInBytes)
        {
            Memory = new byte[memorySizeInBytes];
            Length = memorySizeInBytes;
        }

        public virtual byte ReadByte(int offset)
        {
            return Memory[offset];
        }

        public virtual void WriteByte(int offset, byte value)
        {
            Memory[offset] = value;
        }

        public abstract ushort ReadUInt16(int offset);

        public abstract void WriteUInt16(int offset, ushort value);

        public abstract uint ReadUInt32(int offset);

        public abstract void WriteUInt32(int offset, uint value);

        public virtual void Read(byte[] buffer, int offset, int count, int offsetInMemory)
        {
            Array.Copy(Memory, offsetInMemory, buffer, offset, count);
        }

        public virtual void Write(byte[] buffer, int offset, int count, int offsetInMemory)
        {
            Array.Copy(buffer, offset, Memory, offsetInMemory, count);
        }

        public void Dispose()
        {
            Memory = null;
        }
    }
}
