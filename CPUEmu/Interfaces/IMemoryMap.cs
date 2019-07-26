using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu.Interfaces
{
    public interface IMemoryMap
    {
        byte ReadByte(int offset);
        void WriteByte(int offset, byte value);

        ushort ReadUInt16(int offset);
        void WriteUInt16(int offset, ushort value);

        uint ReadUInt32(int offset);
        void WriteUInt32(int offset, uint value);
    }
}
