using System;
using System.Collections.Generic;
using assembly_aarch32.Support;
using CpuContract;

namespace assembly_aarch32.Instructions
{
    class BlockDataTransferInstruction : IExecutableInstruction<Aarch32CpuState>
    {
        private readonly byte _condition;
        private readonly bool _p;
        private readonly bool _u;
        private readonly bool _s;
        private readonly bool _w;
        private readonly bool _l;
        private readonly byte _rn;
        private readonly uint _list;

        public int Position { get; }

        public int Length => 4;

        private BlockDataTransferInstruction(int position, byte condition, bool p, bool u, bool s, bool w, bool l, byte rn, uint list)
        {
            Position = position;
            _condition = condition;
            _p = p;
            _u = u;
            _s = s;
            _w = w;
            _l = l;
            _rn = rn;
            _list = list;
        }

        public static BlockDataTransferInstruction Parse(int position, byte condition, uint instruction)
        {
            var p = ((instruction >> 24) & 0x1) == 1;
            var u = ((instruction >> 23) & 0x1) == 1;
            var s = ((instruction >> 22) & 0x1) == 1;
            var w = ((instruction >> 21) & 0x1) == 1;
            var l = ((instruction >> 20) & 0x1) == 1;
            var rn = (byte)((instruction >> 16) & 0xF);
            var list = instruction & 0xFFFF;

            return new BlockDataTransferInstruction(position, condition, p, u, s, w, l, rn, list);
        }

        public void Execute(Aarch32CpuState cpuState, DeviceEnvironment env)
        {
            var memoryMap = env.MemoryMap;

            if (!ConditionHelper.CanExecute(cpuState, _condition))
                return;

            var bytesToWrite = 0;
            for (var i = 0; i < 16; i++)
                if (((_list >> i) & 0x1) == 1)
                    bytesToWrite += 4;
            var registerAddress = (int)(cpuState.Registers[_rn] - (_u ? 0 : bytesToWrite - (_p ? -4 : 4)));

            // Registers are always written from lowest to highest
            // From lowest to highest memory address
            var writtenBack = false;
            for (var i = 0; i < 16; i++)
            {
                if (((_list >> i) & 0x1) == 1)
                {
                    if (_p)
                        registerAddress += 4;

                    if (_l)
                        cpuState.Registers[i] = memoryMap.ReadUInt32(registerAddress);
                    else
                    {
                        if (i == 15)
                            // If PC is to be stored, we store the offset of this STM instruction + 12
                            memoryMap.WriteUInt32(registerAddress, (uint)Position + 12);
                        else
                            memoryMap.WriteUInt32(registerAddress, cpuState.Registers[i]);
                    }

                    if (!_p)
                        registerAddress += 4;

                    // A load instruction will always overwrite an updated base register, therefore we don't need to do a write back
                    // A store instruction will write back after the first register was written, no matter which register it was
                    if (!_l && _w && !writtenBack)
                    {
                        writtenBack = true;
                        cpuState.Registers[_rn] = (uint)(cpuState.Registers[_rn] + (_u ? bytesToWrite : -bytesToWrite));
                    }
                }
            }
        }

        public override string ToString()
        {
            var listRegs = new List<int>();
            for (int i = 0; i < 16; i++)
                if ((_list >> i & 0x1) == 1)
                    listRegs.Add(i);

            var result = _l ? "LDM" : "STM";
            result += ConditionHelper.ToString(_condition);

            if (_rn == 13)
            {
                result += _l == _p ? "E" : "F";
                result += _l == _u ? "D" : "A";
            }
            else
            {
                result += _u ? "I" : "D";
                result += _p ? "B" : "A";
            }

            result += " R" + _rn;
            if (_w)
                result += "!";
            result += ", {" + string.Join(",", listRegs) + "}";
            if (_s)
                result += "^";

            return result;
        }

        public void Dispose()
        {

        }
    }
}
