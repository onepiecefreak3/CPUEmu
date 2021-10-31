using CpuContract.Attributes;

namespace CpuContract.Memory
{
    [UniqueIdentifier("BigEndian")]
    public class BigEndianMemoryMapMap : BaseMemoryMapMap
    {
        private byte[] _buffer = new byte[4];

        public BigEndianMemoryMapMap(int memorySize, MemoryRegion payloadRegion, MemoryRegion stackRegion) : base(memorySize, payloadRegion, stackRegion)
        {
        }

        public override ushort ReadUInt16(int offset)
        {
            Read(_buffer, 0, 2, offset);
            return (ushort)((_buffer[0] << 8) | _buffer[1]);
        }

        public override void WriteUInt16(int offset, ushort value)
        {
            _buffer[1] = (byte)value;
            _buffer[0] = (byte)(value >> 8);
            Write(_buffer, 0, 2, offset);
        }

        public override uint ReadUInt32(int offset)
        {
            Read(_buffer, 0, 4, offset);
            return (uint)((_buffer[0] << 24) | (_buffer[1] << 16) | (_buffer[2] << 8) | _buffer[3]);
        }

        public override void WriteUInt32(int offset, uint value)
        {
            _buffer[3] = (byte)value;
            _buffer[2] = (byte)(value >> 8);
            _buffer[1] = (byte)(value >> 16);
            _buffer[0] = (byte)(value >> 24);
            Write(_buffer, 0, 4, offset);
        }
    }
}
