using System;
using CPUEmu.Interfaces;

namespace CPUEmu.Aarch32.Instructions.DataProcessing
{
    abstract class DataProcessingInstruction : IInstruction
    {
        protected abstract bool IsLogical { get; }
        protected byte Rn { get; }
        protected byte Rd { get; }
        protected bool I { get; }
        protected bool S { get; }
        protected uint Operand2 { get; }
        protected byte Condition { get; }

        public int Position { get; }

        public DataProcessingInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd)
        {
            Position = position;
            Condition = condition;

            Rn = rn;
            Rd = rd;
            I = i;
            S = s;
            Operand2 = operand2;
        }

        public void Execute(IEnvironment env)
        {
            var cpuState = env.CpuState;

            if (!ConditionHelper.CanExecute(cpuState, Condition))
                return;

            ExecuteInternal(cpuState, GetOp2Value(cpuState));

            if (Rd == 0xF)
                cpuState.SetRegister("PC", cpuState.GetRegister("R" + Rd));
        }

        protected abstract void ExecuteInternal(ICpuState cpuState, uint operand2Value);

        protected uint GetOp2Value(ICpuState state, bool isDisassemble = false)
        {
            uint shifted;
            bool carry;

            if (I)
            {
                var imm = Operand2 & 0xFF;
                var rot = ((Operand2 >> 8) & 0xF) * 2;

                shifted = BarrelShifter.Instance.Ror(imm, (int)rot, out carry);
                if (rot == 0 && !isDisassemble)
                    carry = Convert.ToBoolean(state.GetFlag("C"));
            }
            else
            {
                var rm = Operand2 & 0xF;
                var shift = (Operand2 >> 4) & 0xFF;

                var stype = (shift >> 1) & 0x3;
                var shiftValue = (shift & 0x1) == 1 ?
                    Convert.ToUInt32(state.GetRegister($"R{(shift >> 4) & 0xF}")) & 0xFF :
                    shift >> 3;
                if ((shift & 0x1) == 0 && (stype == 1 || stype == 2) && shiftValue == 0) shiftValue = 32;

                shifted = BarrelShifter.Instance.ShiftByType((BarrelShifter.ShiftType)stype, Convert.ToUInt32(state.GetRegister($"R{rm}")), (int)shiftValue, out carry);
                if (shiftValue == 0 && !isDisassemble)
                    carry = Convert.ToBoolean(state.GetFlag("C"));
            }

            if (S && IsLogical && !isDisassemble)
                state.SetFlag("C", carry);

            return shifted;
        }

        protected void SetFlagsArithmethic(ICpuState cpuState, uint result, uint value, uint value2, bool add, bool carryIn)
        {
            SetFlagsLogical(cpuState, result);

            var c = !carryIn ? result < value : result <= value;
            var v = !add ?
                value >> 31 != value2 >> 31 && value >> 31 != result >> 31 :
                value >> 31 == value2 >> 31 && value >> 31 != result >> 31;

            cpuState.SetFlag("C", c);
            cpuState.SetFlag("V", v);
        }

        protected void SetFlagsLogical(ICpuState cpuState, uint result)
        {
            var z = result == 0;
            var n = result >> 31 == 1;

            cpuState.SetFlag("Z", z);
            cpuState.SetFlag("N", n);
        }
    }
}
