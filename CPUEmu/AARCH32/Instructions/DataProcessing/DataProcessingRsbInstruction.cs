namespace CPUEmu.Aarch32.Instructions.DataProcessing
{
    class DataProcessingRsbInstruction : DataProcessingInstruction
    {
        protected override bool IsLogical => false;

        public DataProcessingRsbInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd) : 
            base(position, condition, i, s, operand2, rn, rd)
        {
        }

        protected override void ExecuteInternal(Aarch32CpuState cpuState, uint operand2Value)
        {
            var rn = cpuState.Registers[Rn];
            var rdNewValue = operand2Value - rn;

            cpuState.Registers[Rd] = rdNewValue;
            if (S)
                SetFlagsArithmethic(cpuState, rdNewValue, operand2Value, rn, false, true);
        }

        protected override string ToStringInternal()
        {
            var result = "RSB";
            if (S)
                result += "S";
            result += ConditionHelper.ToString(Condition);
            result += " R" + Rd + ", ";
            result += "R" + Rn + ", ";

            return result;
        }
    }
}
