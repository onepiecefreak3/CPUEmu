﻿using architecture_avr.Support;
using CpuContract;

namespace architecture_avr.Instructions.ArithmeticLogical
{
    class SubInstruction : BaseInstruction
    {
        private int _rd;
        private int _rr;
        private bool _c;
        private bool _imm;

        public SubInstruction(int position, int rd, int rr, bool c, bool imm = false) : base(position, 2)
        {
            _rd = rd;
            _rr = rr;
            _c = c;
            _imm = imm;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var cv = cpuState.C ? 1 : 0;
            var v1 = cpuState.Registers[_rd];
            var v2 = _imm ? (byte)_rr : cpuState.Registers[_rr];
            var res = (byte)(v1 - v2 - (_c ? cv : 0));

            cpuState.H = ((~v1 & 0x4) & (v2 & 0x4) + (v2 & 0x4) & (res & 0x4) + (res & 0x4) & (~v1 & 0x4)) != 0;
            cpuState.N = (res & 0x80) == 0x80;
            cpuState.Z = _c ? res == 0 && cpuState.Z : res == 0;
            cpuState.V = FlagHelper.IsTwoComplementCarry(v1, v2, res, true);
            cpuState.C = v2 + (_c ? cv : 0) > v1;
        }

        public override string ToString()
        {
            return $"{(_c ? "SBC" : "SUB")}{(_imm ? "I" : "")} R{_rd}, {(_imm ? "" : "R")}{_rr}";
        }
    }
}
