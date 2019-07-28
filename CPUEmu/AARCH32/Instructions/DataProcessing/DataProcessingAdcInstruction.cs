namespace CPUEmu.Aarch32.Instructions.DataProcessing
{
    class DataProcessingAdcInstruction : DataProcessingInstruction
    {
        protected override bool IsLogical => false;

        public DataProcessingAdcInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd) :
            base(position, condition, i, s, operand2, rn, rd)
        {
        }

        protected override void ExecuteInternal(Aarch32CpuState cpuState, uint operand2Value)
        {
            var rn = cpuState.Registers[Rn];
            var rdNewValue = rn + operand2Value + (cpuState.C ? 1 : 0);

            cpuState.Registers[Rd] = (uint)rdNewValue;
            if (S)
                SetFlagsArithmethic(cpuState, (uint)rdNewValue, rn, operand2Value, true, cpuState.C);
        }

        protected override string ToStringInternal()
        {
            var result = "ADC";
            if (S)
                result += "S";
            result += ConditionHelper.ToString(Condition);
            result += " R" + Rd + ", ";
            result += "R" + Rn + ", ";

            return result;
        }
    }
}
