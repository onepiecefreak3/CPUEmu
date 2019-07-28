namespace CPUEmu.Defaults
{
    class LittleEndianMemoryMap : DefaultMemoryMap
    {
        public LittleEndianMemoryMap(int memorySizeInBytes):base(memorySizeInBytes)
        {
        }

        public override ushort ReadUInt16(int offset)
        {
            return (ushort)(Memory[offset] | Memory[offset + 1] << 8);
        }

        public override void WriteUInt16(int offset, ushort value)
        {
            Memory[offset] = (byte)(value & 0xFF);
            Memory[offset + 1] = (byte)((value >> 8) & 0xFF);
        }

        public override uint ReadUInt32(int offset)
        {
            return (uint)(Memory[offset] | Memory[offset + 1] << 8 | Memory[offset + 2] << 16 | Memory[offset + 3] << 24);
        }

        public override void WriteUInt32(int offset, uint value)
        {
            Memory[offset] = (byte)(value & 0xFF);
            Memory[offset + 1] = (byte)((value >> 8) & 0xFF);
            Memory[offset + 2] = (byte)((value >> 16) & 0xFF);
            Memory[offset + 3] = (byte)((value >> 24) & 0xFF);
        }
    }
}
