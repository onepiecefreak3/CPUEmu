namespace CPUEmu.Aarch32.Instructions.DataProcessing
{
    class DataProcessingCmnInstruction : DataProcessingInstruction
    {
        protected override bool IsLogical => false;

        public DataProcessingCmnInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd) : 
            base(position, condition, i, s, operand2, rn, rd)
        {
        }

        protected override void ExecuteInternal(Aarch32CpuState cpuState, uint operand2Value)
        {
            var rn = cpuState.Registers[Rn];
            var res = rn + operand2Value;

            SetFlagsArithmethic(cpuState, res, rn, operand2Value, true, false);
        }

        protected override string ToStringInternal()
        {
            var result = "CMN";
            result += ConditionHelper.ToString(Condition);
            result += " R" + Rn + ", ";

            return result;
        }
    }
}
