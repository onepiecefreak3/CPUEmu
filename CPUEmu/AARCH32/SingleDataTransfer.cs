using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu
{
    public partial class AARCH32
    {
        private void SingleDataTransfer(uint instruction)
        {
            var desc = DescribeSingleDataTransfer(instruction);

            long baseAddr = _reg[desc.rn];
            if (desc.p)
                if (desc.u)
                    baseAddr += desc.offset;
                else
                    baseAddr -= desc.offset;

            if (desc.l)
            {
                if (desc.b)
                    _reg[desc.rd] = _mem[baseAddr];
                else
                    _reg[desc.rd] = ReadUInt32(baseAddr);

                if (desc.rd == 0xF)
                    SetPC(_reg[desc.rd]);
            }
            else
            {
                if (desc.b)
                    _mem[baseAddr] = (byte)_reg[desc.rd];
                else
                {
                    WriteUInt32(baseAddr, _reg[desc.rd]);
                }
            }

            if (!desc.p)
                if (desc.u)
                    baseAddr += desc.offset;
                else
                    baseAddr -= desc.offset;

            if (desc.w)
                _reg[desc.rn] = (uint)baseAddr;
        }

        private string DisassembleSingleDataTransfer(uint instruction)
        {
            var desc = DescribeSingleDataTransfer(instruction);

            if (desc.offset != 0)
                return $"{(desc.l ? "LDR" : "STR")}{(desc.b ? "B" : "")}{_currentCondition} R{desc.rd}, [R{desc.rn}{(desc.i ? $", #{desc.offset}]" : (desc.p ? $", #{desc.offset}]!" : $"], #{desc.offset}"))}";
            else
                return $"{(desc.l ? "LDR" : "STR")}{(desc.b ? "B" : "")}{_currentCondition} R{desc.rd}, [R{desc.rn}]{(desc.p ? "!" : "")}";
        }

        private SingleDataTransferDescriptor DescribeSingleDataTransfer(uint instruction)
        {
            var res = new SingleDataTransferDescriptor
            {
                i = ((instruction >> 25) & 0x1) == 1,
                p = ((instruction >> 24) & 0x1) == 1,
                u = ((instruction >> 23) & 0x1) == 1,
                b = ((instruction >> 22) & 0x1) == 1,
                w = ((instruction >> 21) & 0x1) == 1,
                l = ((instruction >> 20) & 0x1) == 1,
                rn = (instruction >> 16) & 0xF,
                rd = (instruction >> 12) & 0xF,
                offset = instruction & 0xFFF
            };

            if (res.i)
            {
                var rm = res.offset & 0xF;
                var shift = (res.offset >> 4) & 0xFF;

                var stype = (shift >> 1) & 0x3;
                var shiftValue = (shift & 0x1) == 1 ? _reg[(shift >> 4) & 0xF] & 0xFF : shift >> 3;

                res.offset = _shifter.ShiftByType((BarrelShifter.ShiftType)stype, _reg[res.offset & 0xF], (int)shiftValue, out _c);
            }

            return res;
        }
        private class SingleDataTransferDescriptor
        {
            public bool i;
            public bool p;
            public bool u;
            public bool b;
            public bool w;
            public bool l;
            public long rn;
            public long rd;
            public long offset;
        }
    }
}
