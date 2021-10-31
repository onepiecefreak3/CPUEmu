using assembly_aarch32.Support;

namespace assembly_aarch32.Instructions.DataProcessing
{
    class DataProcessingAddInstruction : DataProcessingInstruction
    {
        protected override bool IsLogical => false;

        public DataProcessingAddInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd) :
            base(position, condition, i, s, operand2, rn, rd)
        {
        }

        protected override void ExecuteInternal(Aarch32CpuState cpuState, uint operand2Value)
        {
            var rn = cpuState.Registers[Rn];
            var rdNewValue = rn + operand2Value;

            cpuState.Registers[Rd] = rn+operand2Value;
            if (S)
                SetFlagsArithmethic(cpuState, rdNewValue, rn, operand2Value, true, false);
        }

        protected override string ToStringInternal()
        {
            var result = "ADD";
            if (S)
                result += "S";
            result += ConditionHelper.ToString(Condition);
            result += " R" + Rd + ", ";
            result += "R" + Rn + ", ";

            return result;
        }
    }
}
