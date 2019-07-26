using CPUEmu.Interfaces;

namespace CPUEmu
{
    class LittleEndianMemoryMap : IMemoryMap
    {
        private readonly byte[] _memory;

        public LittleEndianMemoryMap(int memorySizeInBytes)
        {
            _memory = new byte[memorySizeInBytes];
        }

        public byte ReadByte(int offset)
        {
            return _memory[offset];
        }

        public void WriteByte(int offset, byte value)
        {
            _memory[offset] = value;
        }

        public ushort ReadUInt16(int offset)
        {
            return (ushort)(_memory[offset + 1] | _memory[offset] << 8);
        }

        public void WriteUInt16(int offset, ushort value)
        {
            _memory[offset + 1] = (byte)(value & 0xFF);
            _memory[offset] = (byte)((value >> 8) & 0xFF);
        }

        public uint ReadUInt32(int offset)
        {
            return (uint)(_memory[offset + 3] | _memory[offset + 2] << 8 | _memory[offset + 1] << 16 | _memory[offset] << 24);
        }

        public void WriteUInt32(int offset, uint value)
        {
            _memory[offset + 3] = (byte)(value & 0xFF);
            _memory[offset + 2] = (byte)((value >> 8) & 0xFF);
            _memory[offset + 1] = (byte)((value >> 16) & 0xFF);
            _memory[offset] = (byte)((value >> 24) & 0xFF);
        }
    }
}
