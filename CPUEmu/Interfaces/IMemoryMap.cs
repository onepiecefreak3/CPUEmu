using System;

namespace CPUEmu.Interfaces
{
    public interface IMemoryMap : IDisposable
    {
        int Length { get; }

        void Clear();

        byte ReadByte(int offset);
        void WriteByte(int offset, byte value);

        ushort ReadUInt16(int offset);
        void WriteUInt16(int offset, ushort value);

        uint ReadUInt32(int offset);
        void WriteUInt32(int offset, uint value);

        void Read(byte[] buffer, int offset, int count, int offsetInMemory);
        void Write(byte[] buffer, int offset, int count, int offsetInMemory);
    }
}
