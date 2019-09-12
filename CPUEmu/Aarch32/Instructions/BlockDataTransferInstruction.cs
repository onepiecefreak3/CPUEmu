using System;
using System.Collections.Generic;
using CPUEmu.Interfaces;

namespace CPUEmu.Aarch32.Instructions
{
    class BlockDataTransferInstruction : IInstruction
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

        public static IInstruction Parse(int position, byte condition, uint instruction)
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

        public void Execute(IEnvironment env)
        {
            var cpuState = env.CpuState as Aarch32CpuState;
            if (cpuState == null)
                throw new InvalidOperationException("Unknown cpu state.");
            var memoryMap = env.MemoryMap;

            if (!ConditionHelper.CanExecute(cpuState, _condition))
                return;

            var bytesToWrite = 0;
            for (var i = 0; i < 16; i++)
                if (((_list >> i) & 0x1) == 1)
                    bytesToWrite += 4;
            var address = (int)(cpuState.Registers[_rn] - (_u ? 0 : bytesToWrite));

            var writtenBack = false;
            for (var i = 0; i < 16; i++)
            {
                if (((_list >> i) & 0x1) == 1)
                {
                    if (_p || !_u)
                        address += 4;

                    if (_l)
                        cpuState.Registers[i] = memoryMap.ReadUInt32(address);
                    else
                        memoryMap.WriteUInt32(address, cpuState.Registers[i]);

                    if (!_p || _u)
                        address += 4;

                    if (_w && !writtenBack)
                    {
                        writtenBack = true;
                        var rnValue = cpuState.Registers[_rn];
                        cpuState.Registers[_rn] = (uint)(rnValue + bytesToWrite);
                        // TODO: Check increment/decrement
                        //_reg[desc.rn] -= bytesToWrite;
                        //_reg[desc.rn] += bytesToWrite;
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
            result += _l == _p ? "E" : "F";
            result += _l == _u ? "D" : "A";
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
