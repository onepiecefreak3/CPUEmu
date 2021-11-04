namespace CpuContract.Memory
{
    public interface IMemoryRegion
    {
        int Address { get; }
        int Length { get; }

        byte this[int index]
        {
            get;
            set;
        }

        byte ReadByte(int offset);
        void WriteByte(int offset, byte value);

        ushort ReadUInt16(int offset);
        void WriteUInt16(int offset, ushort value);

        uint ReadUInt32(int offset);
        void WriteUInt32(int offset, uint value);

        void Read(byte[] buffer, int offset, int size, int offsetInMemory);
        void Write(byte[] buffer, int offset, int size, int offsetInMemory);

        void Clear(int offsetInMemory, int count);
        void ClearAll();
    }
}
