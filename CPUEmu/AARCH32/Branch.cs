using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu
{
    public partial class AARCH32
    {
        private void Branch(uint instruction)
        {
            var desc = DescribeBranch(instruction);

            if (desc.l)
                _lr = _pc - 4;

            SetPC((uint)((int)_pc + (int)desc.offset));
        }

        private string DisassembleBranch(uint instruction)
        {
            var desc = DescribeBranch(instruction);

            return $"B{(desc.l ? "L" : "")}{_currentCondition} 0x{_currentInstrOffset + 8 + desc.offset:X2}";
        }

        private BranchDescriptor DescribeBranch(uint instruction)
        {
            var offset = (instruction & 0xFFFFFF) << 2;

            //sign extend offset
            var sign = (offset >> 25) & 0x1;
            for (int i = 26; i < 32; i++) offset |= sign << i;

            return new BranchDescriptor
            {
                offset = offset,
                l = ((instruction >> 24) & 0x1) == 1
            };
        }
        private class BranchDescriptor
        {
            public uint offset;
            public bool l;
        }

        private void BranchExchange(uint instruction)
        {

        }
    }
}
