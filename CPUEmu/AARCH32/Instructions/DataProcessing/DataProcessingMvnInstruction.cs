using CPUEmu.Interfaces;

namespace CPUEmu.Aarch32.Instructions.DataProcessing
{
    class DataProcessingMvnInstruction : DataProcessingInstruction
    {
        protected override bool IsLogical => true;

        public DataProcessingMvnInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd) : 
            base(position, condition, i, s, operand2, rn, rd)
        {
        }

        protected override void ExecuteInternal(ICpuState cpuState, uint operand2Value)
        {
            cpuState.SetRegister($"R{Rd}", ~operand2Value);
            if (S)
                SetFlagsLogical(cpuState, ~operand2Value);
        }

        public override string ToString()
        {
            return $"MVN{(S ? "S" : "")}{ConditionHelper.ToString(Condition)} R{Rd}, {(I ? $"#{GetOp2Value(null, true)}" : $"R{Operand2 & 0xF}")}";
        }
    }
}
