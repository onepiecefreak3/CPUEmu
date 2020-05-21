using System;
using System.Collections.Generic;
using System.Text;
using CpuContract;

namespace assembly_aarch64.Instructions.DataProcessing
{
    class ImmediateAddSubDataProcessing : IInstruction
    {
        private int _rn;
        private int _rd;
        private int _imm;
        private bool _sf;
        private bool _op;
        private bool _s;
        private byte _shift;

        private int _finalImm;
        private bool _dzr;

        public int Position { get; }

        private ImmediateAddSubDataProcessing(int rn, int rd, int imm, bool sf, bool op, bool s, byte shift)
        {
            _rn = rn;
            _rd = rd;
            _imm = imm;
            _sf = sf;
            _op = op;
            _s = s;
            _shift = shift;

            _finalImm = shift == 1 ? imm << 12 : imm;
            _dzr = rd == 31 && s;
        }

        public static IInstruction Parse(uint instruction, int position)
        {
            var rd = (int)instruction & 0x1F;
            var rn = (int)(instruction >> 5) & 0x1F;
            var imm = (int)(instruction >> 10) & 0xFFF;
            var shift = (byte)((instruction >> 22) & 0x3);

            var s = ((instruction >> 29) & 0x1) == 1;
            var op = ((instruction >> 30) & 0x1) == 1;
            var sf = ((instruction >> 31) & 0x1) == 1;

            return new ImmediateAddSubDataProcessing(rn, rd, imm, sf, op, s, shift);
        }

        public void Execute(IExecutionEnvironment env)
        {
            if (!(env.CpuState is Aarch64CpuState armCpuState))
                throw new InvalidOperationException("Unknown cpu state.");

            var operand1 = _rn == 31 ? 0 : armCpuState.Registers[_rn];

            ulong result;
            if (_op)
            {
                if (_sf)
                    result = operand1 - (ulong)_finalImm;
                else
                    result = (uint)operand1 - (uint)_finalImm;
            }
            else
            {
                if (_sf)
                    result = operand1 + (ulong)_finalImm;
                else
                    result = operand1 + (ulong)_finalImm;
            }

            if (!_dzr)
                armCpuState.Registers[_rd] = result;

            if (_s)
                Support.SetFlagsArithmethic(armCpuState, result, operand1, (ulong)_finalImm, !_op, armCpuState.C, _sf);
        }

        public override string ToString()
        {
            // Alias MOV (to/from SP)
            if (!_op && (_rd == 31 || _rn == 31) && _shift == 0 && _imm == 0)
                return "MOV " + (_rd == 31 ? "SP" : (_sf ? "X" : "W") + _rd) + ", " +
                       (_rn == 31 ? "SP" : (_sf ? "X" : "W") + _rn);

            var result = _op ? "SUB" : "ADD";
            if (_s)
                result += "S";

            result += " " + (_sf ? "X" : "W");
            result += _rd == 31 ? "SP" : _rd.ToString();

            result += ", " + (_sf ? "X" : "W");
            result += _rd == 31 ? "SP" : _rn.ToString();

            result += ", #0x" + _imm.ToString("X");
            if (_shift == 1)
                result += ", LSL#12";

            return result;
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
