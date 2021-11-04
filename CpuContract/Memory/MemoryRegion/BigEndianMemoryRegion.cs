namespace CpuContract.Memory.MemoryRegion
{
    public class BigEndianMemoryRegion : BaseMemoryRegion
    {
        public BigEndianMemoryRegion(byte[] memory, int address, int length) : base(memory, address, length)
        {
        }

        protected override ushort ReadUInt16(byte[] buffer)
        {
            return (ushort)((buffer[0] << 8) | buffer[1]);
        }

        protected override void WriteUInt16(byte[] buffer, ushort value)
        {
            buffer[1] = (byte)value;
            buffer[0] = (byte)(value >> 8);
        }

        protected override uint ReadUInt32(byte[] buffer)
        {
            return (uint)((buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3]);
        }

        protected override void WriteUInt32(byte[] buffer, uint value)
        {
            buffer[3] = (byte)value;
            buffer[2] = (byte)(value >> 8);
            buffer[1] = (byte)(value >> 16);
            buffer[0] = (byte)(value >> 24);
        }
    }
}
