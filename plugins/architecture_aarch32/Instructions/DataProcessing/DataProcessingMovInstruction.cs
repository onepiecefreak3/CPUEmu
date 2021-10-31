using assembly_aarch32.Support;

namespace assembly_aarch32.Instructions.DataProcessing
{
    class DataProcessingMovInstruction : DataProcessingInstruction
    {
        protected override bool IsLogical => true;

        public DataProcessingMovInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd) : 
            base(position, condition, i, s, operand2, rn, rd)
        {
        }

        protected override void ExecuteInternal(Aarch32CpuState cpuState, uint operand2Value)
        {
            cpuState.Registers[Rd] = operand2Value;
            if (S)
                SetFlagsLogical(cpuState, operand2Value);
        }

        protected override string ToStringInternal()
        {
            var result = "MOV";
            if (S)
                result += "S";
            result += ConditionHelper.ToString(Condition);
            result += " R" + Rd + ", ";

            return result;
        }
    }
}
