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
            switch (env.CpuState)
            {
                case Aarch32CpuState armCpuState:
                    if (!ConditionHelper.CanExecute(armCpuState, Condition))
                        return;

                    ExecuteInternal(armCpuState, GetOp2Value(armCpuState));

                    // TODO: Legacy code that called a method to reset the instruction queue to the new PC after it was changed
                    //if (Rd == 0xF)
                    //    armCpuState.PC = armCpuState.Registers[Rd];
                    break;
                default:
                    throw new InvalidOperationException("Unknown cpu state.");
            }
        }

        protected abstract void ExecuteInternal(Aarch32CpuState cpuState, uint operand2Value);

        protected uint GetOp2Value(Aarch32CpuState state, bool isDisassemble = false)
        {
            uint shifted;
            bool carry;

            if (I)
            {
                var imm = Operand2 & 0xFF;
                var rot = ((Operand2 >> 8) & 0xF) * 2;

                shifted = BarrelShifter.Instance.Ror(imm, (int)rot, out carry);
                if (rot == 0 && !isDisassemble)
                    carry = state.C;
            }
            else
            {
                var rm = Operand2 & 0xF;
                var shift = (Operand2 >> 4) & 0xFF;

                var stype = (shift >> 1) & 0x3;
                var shiftValue = (shift & 0x1) == 1 ?
                    state.Registers[(shift >> 4) & 0xF] & 0xFF :
                    shift >> 3;
                if ((shift & 0x1) == 0 && (stype == 1 || stype == 2) && shiftValue == 0) shiftValue = 32;

                shifted = BarrelShifter.Instance.ShiftByType((ShiftType)stype, state.Registers[rm], (int)shiftValue, out carry);
                if (shiftValue == 0 && !isDisassemble)
                    carry = state.C;
            }

            if (S && IsLogical && !isDisassemble)
                state.C = carry;

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

        public override string ToString()
        {
            var result = ToStringInternal();

            var isRelevantShift = ((Operand2 >> 4) & 0x1) == 1 || Operand2 >> 7 != 0;
            var shiftType = (ShiftType)(Operand2 >> 5 & 0x3);

            if (I)
                result += $"#0x{GetOp2Value(null, true):X}";
            else
            {
                result += "R" + (Operand2 & 0xF);
                if (isRelevantShift)
                {
                    result += ", ";
                    result += shiftType;
                    if (((Operand2 >> 4) & 0x1) == 1)
                        result += " R" + (Operand2 >> 8);
                    else
                        result += "#" + (Operand2 >> 7);
                }
            }

            return result;
        }

        protected abstract string ToStringInternal();

        public void Dispose()
        {

        }
    }
}
