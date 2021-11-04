namespace CpuContract.Memory.MemoryRegion
{
    public class LittleEndianMemoryRegion : BaseMemoryRegion
    {
        public LittleEndianMemoryRegion(byte[] memory, int address, int length) : base(memory, address, length)
        {
        }

        protected override ushort ReadUInt16(byte[] buffer)
        {
            return (ushort)((buffer[1] << 8) | buffer[0]);
        }

        protected override void WriteUInt16(byte[] buffer, ushort value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
        }

        protected override uint ReadUInt32(byte[] buffer)
        {
            return (uint)((buffer[3] << 24) | (buffer[2] << 16) | (buffer[1] << 8) | buffer[0]);
        }

        protected override void WriteUInt32(byte[] buffer, uint value)
        {
            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
        }
    }
}
