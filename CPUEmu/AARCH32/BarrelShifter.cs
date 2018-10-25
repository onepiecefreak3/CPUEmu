using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu
{
    public partial class AARCH32
    {
        private uint ROR(uint value, int rot) => (uint)((value >> rot) | ((value & ((1 << rot) - 1)) << (32 - rot)));

        private uint Shift(uint value, uint type, uint count, bool s)
        {
            switch (type)
            {
                //LSL
                case 0:
                    if (count != 0 && s)
                        _c = (((int)value >> (int)(32 - count)) & 0x1) == 1;
                    return value << (int)count;

                //LSR
                case 1:
                    if (count != 0 && s)
                        _c = (((int)value >> (int)(count - 1)) & 0x1) == 1;
                    return value >> (int)count;

                //ASR
                case 2:
                    if (count != 0 && s)
                        _c = (((int)value >> (int)(count - 1)) & 0x1) == 1;
                    return (uint)((int)value >> (int)count);

                //ROR
                case 3:
                    if (count != 0 && s)
                        _c = (((int)value >> (int)(count - 1)) & 0x1) == 1;
                    return ROR(value, (int)count);

                default:
                    return 0;
            }
        }
    }
}
