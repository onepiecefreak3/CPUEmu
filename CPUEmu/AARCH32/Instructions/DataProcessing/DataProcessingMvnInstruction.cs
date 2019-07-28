namespace CPUEmu.Aarch32.Instructions.DataProcessing
{
    class DataProcessingMvnInstruction : DataProcessingInstruction
    {
        protected override bool IsLogical => true;

        public DataProcessingMvnInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd) : 
            base(position, condition, i, s, operand2, rn, rd)
        {
        }

        protected override void ExecuteInternal(Aarch32CpuState cpuState, uint operand2Value)
        {
            cpuState.Registers[Rd] = ~operand2Value;
            if (S)
                SetFlagsLogical(cpuState, ~operand2Value);
        }

        protected override string ToStringInternal()
        {
            var result = "MVN";
            if (S)
                result += "S";
            result += ConditionHelper.ToString(Condition);
            result += " R" + Rd + ", ";

            return result;
        }
    }
}
