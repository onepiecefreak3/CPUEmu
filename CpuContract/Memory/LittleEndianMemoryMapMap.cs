﻿using CpuContract.Attributes;

namespace CpuContract.Memory
{
    [UniqueIdentifier("LittleEndian")]
    public class LittleEndianMemoryMapMap : BaseMemoryMapMap
    {
        private readonly byte[] _buffer = new byte[4];

        public LittleEndianMemoryMapMap(int memorySize, int payloadAddress, int payloadSize, int stackAddress, int stackSize) :
            base(memorySize, new MemoryRegion(payloadAddress,payloadSize), new MemoryRegion(stackAddress,stackSize))
        {
        }

        public override ushort ReadUInt16(int offset)
        {
            Read(_buffer, 0, 2, offset);
            return (ushort)((_buffer[1] << 8) | _buffer[0]);
        }

        public override void WriteUInt16(int offset, ushort value)
        {
            _buffer[0] = (byte)value;
            _buffer[1] = (byte)(value >> 8);
            Write(_buffer, 0, 2, offset);
        }

        public override uint ReadUInt32(int offset)
        {
            Read(_buffer, 0, 4, offset);
            return (uint)((_buffer[3] << 24) | (_buffer[2] << 16) | (_buffer[1] << 8) | _buffer[0]);
        }

        public override void WriteUInt32(int offset, uint value)
        {
            _buffer[0] = (byte)value;
            _buffer[1] = (byte)(value >> 8);
            _buffer[2] = (byte)(value >> 16);
            _buffer[3] = (byte)(value >> 24);
            Write(_buffer, 0, 4, offset);
        }
    }
}