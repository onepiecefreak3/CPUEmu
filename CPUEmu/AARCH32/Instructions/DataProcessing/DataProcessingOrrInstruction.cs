using System;
using CPUEmu.Interfaces;

namespace CPUEmu.AARCH32.Instructions.DataProcessing
{
    class DataProcessingOrrInstruction : DataProcessingInstruction
    {
        protected override bool IsLogical => true;

        public DataProcessingOrrInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd) :
            base(position, condition, i, s, operand2, rn, rd)
        {
        }

        protected override void ExecuteInternal(ICpuState cpuState, uint operand2Value)
        {
            var rn = Convert.ToUInt32(cpuState.GetRegister($"R{Rn}"));
            var rdNewValue = rn | operand2Value;

            cpuState.SetRegister($"R{Rd}", rdNewValue);
            if (S)
                SetFlagsLogical(cpuState, rdNewValue);
        }

        public override string ToString()
        {
            return $"ORR{(S ? "S" : "")}{ConditionHelper.ToString(Condition)} R{Rd}, R{Rn}, {(I ? $"#{GetOp2Value(null, true)}" : $"R{Operand2 & 0xF}")}";
        }
    }
}
