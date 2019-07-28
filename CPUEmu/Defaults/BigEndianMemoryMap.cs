namespace CPUEmu.Defaults
{
    class BigEndianMemoryMap : DefaultMemoryMap
    {
        public BigEndianMemoryMap(int memorySizeInBytes) : base(memorySizeInBytes)
        {
        }

        public override ushort ReadUInt16(int offset)
        {
            return (ushort)(Memory[offset + 1] | Memory[offset] << 8);
        }

        public override void WriteUInt16(int offset, ushort value)
        {
            Memory[offset + 1] = (byte)(value & 0xFF);
            Memory[offset] = (byte)((value >> 8) & 0xFF);
        }

        public override uint ReadUInt32(int offset)
        {
            return (uint)(Memory[offset + 3] | Memory[offset + 2] << 8 | Memory[offset + 1] << 16 | Memory[offset] << 24);
        }

        public override void WriteUInt32(int offset, uint value)
        {
            Memory[offset + 3] = (byte)(value & 0xFF);
            Memory[offset + 2] = (byte)((value >> 8) & 0xFF);
            Memory[offset + 1] = (byte)((value >> 16) & 0xFF);
            Memory[offset] = (byte)((value >> 24) & 0xFF);
        }
    }
}
