using System;
using CPUEmu.Interfaces;

namespace CPUEmu.AARCH32.Instructions.DataProcessing
{
    class DataProcessingCmnInstruction : DataProcessingInstruction
    {
        protected override bool IsLogical => false;

        public DataProcessingCmnInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd) : 
            base(position, condition, i, s, operand2, rn, rd)
        {
        }

        protected override void ExecuteInternal(ICpuState cpuState, uint operand2Value)
        {
            var rn = Convert.ToUInt32(cpuState.GetRegister($"R{Rn}"));
            var res = rn + operand2Value;

            SetFlagsArithmethic(cpuState, res, rn, operand2Value, true, false);
        }

        public override string ToString()
        {
            return $"CMN{ConditionHelper.ToString(Condition)} R{Rn}, {(I ? $"#{GetOp2Value(null, true)}" : $"R{Operand2 & 0xF}")}";
        }
    }
}
