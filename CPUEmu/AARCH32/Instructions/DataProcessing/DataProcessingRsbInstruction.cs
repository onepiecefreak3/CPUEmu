using System;
using CPUEmu.Interfaces;

namespace CPUEmu.Aarch32.Instructions.DataProcessing
{
    class DataProcessingRsbInstruction : DataProcessingInstruction
    {
        protected override bool IsLogical => false;

        public DataProcessingRsbInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd) : 
            base(position, condition, i, s, operand2, rn, rd)
        {
        }

        protected override void ExecuteInternal(ICpuState cpuState, uint operand2Value)
        {
            var rn = Convert.ToUInt32(cpuState.GetRegister($"R{Rn}"));
            var rdNewValue = operand2Value - rn;

            cpuState.SetRegister($"R{Rd}", rdNewValue);
            if (S)
                SetFlagsArithmethic(cpuState, rdNewValue, operand2Value, rn, false, true);
        }

        public override string ToString()
        {
            return $"RSB{(S ? "S" : "")}{ConditionHelper.ToString(Condition)} R{Rd}, {(I ? $"#{GetOp2Value(null, true)}" : $"R{Operand2 & 0xF}")}, R{Rn}";
        }
    }
}
