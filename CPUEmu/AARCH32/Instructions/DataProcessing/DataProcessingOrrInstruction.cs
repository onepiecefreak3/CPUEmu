namespace CPUEmu.Aarch32.Instructions.DataProcessing
{
    class DataProcessingOrrInstruction : DataProcessingInstruction
    {
        protected override bool IsLogical => true;

        public DataProcessingOrrInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd) :
            base(position, condition, i, s, operand2, rn, rd)
        {
        }

        protected override void ExecuteInternal(Aarch32CpuState cpuState, uint operand2Value)
        {
            var rn = cpuState.Registers[Rn];
            var rdNewValue = rn | operand2Value;

            cpuState.Registers[Rd] = rdNewValue;
            if (S)
                SetFlagsLogical(cpuState, rdNewValue);
        }

        protected override string ToStringInternal()
        {
            var result = "ORR";
            if (S)
                result += "S";
            result += ConditionHelper.ToString(Condition);
            result += " R" + Rd + ", ";
            result += "R" + Rn + ", ";

            return result;
        }
    }
}
