using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu
{
    public partial class AARCH32
    {
        private void BlockDataTransfer(uint instruction)
        {
            var desc = DescribeBlockDataTransfer(instruction);

            if (desc.u)
                IncrementAddressing(desc);
            else
                DecrementAddressing(desc);

            //TODO: Finish Block Data Transfer

            long address = _reg[desc.rn];

            for (int i = 0; i < 16; i++)
            {
                if (((desc.list >> i) & 0x1) == 1)
                {
                    if (desc.p || !desc.u)
                        address += 4;

                    if (desc.l)
                    {
                        _reg[i] = ReadUInt32((uint)address);
                        if (i == 15)
                            SetPC(_reg[i]);
                    }
                    else
                        WriteUInt32((uint)address, _reg[i]);

                    if (!desc.p || !desc.u)
                        address += 4;
                }
            }
        }

        private void IncrementAddressing(BlockDataTransferDescriptor desc)
        {
            var address = _reg[desc.rn];

            for (int i = 0; i < 16; i++)
            {
                if (((desc.list >> i) & 0x1) == 1)
                {
                    if (desc.p)
                        address += 4;

                    if (desc.l)
                        _reg[i] = ReadUInt32(address);
                    else
                        WriteUInt32(address, _reg[i]);

                    if (!desc.p)
                        address += 4;
                }
            }
        }

        private void DecrementAddressing(BlockDataTransferDescriptor desc)
        {
            var spaceToWrite = Enumerable.Range(0, 16).Aggregate(0, (a, b) => a + (((desc.list >> b) & 0x1) == 1 ? 4 : 0));
            var address = _reg[desc.rn] - spaceToWrite;

            for (int i = 0; i < 16; i++)
            {
                if (((desc.list >> i) & 0x1) == 1)
                {
                    if (desc.p)
                    {
                        address += 4;
                        if (desc.w)
                            _reg[desc.rn] -= 4;
                    }

                    if (desc.l)
                        _reg[i] = ReadUInt32(address);
                    else
                        WriteUInt32(address, _reg[i]);

                    if (!desc.p)
                    {
                        address += 4;
                        if (desc.w)
                            _reg[desc.rn] -= 4;
                    }
                }
            }
        }

        private string DisassembleBlockDataTransfer(uint instruction)
        {
            var desc = DescribeBlockDataTransfer(instruction);

            return "";
        }

        private BlockDataTransferDescriptor DescribeBlockDataTransfer(uint instruction)
        {
            var res = new BlockDataTransferDescriptor
            {
                p = ((instruction >> 24) & 0x1) == 1,
                u = ((instruction >> 23) & 0x1) == 1,
                s = ((instruction >> 22) & 0x1) == 1,
                w = ((instruction >> 21) & 0x1) == 1,
                l = ((instruction >> 20) & 0x1) == 1,
                rn = (instruction >> 16) & 0xF,
                list = instruction & 0xFFFF
            };

            return res;
        }
        private class BlockDataTransferDescriptor
        {
            public bool p;
            public bool u;
            public bool s;
            public bool w;
            public bool l;
            public long rn;
            public long list;
        }
    }
}
