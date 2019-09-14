using assembly_aarch32.Support;
using CpuContract;

namespace assembly_aarch32.Instructions.DataProcessing
{
    class DataProcessingAndInstruction : DataProcessingInstruction
    {
        protected override bool IsLogical => true;

        public DataProcessingAndInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd) :
            base(position, condition, i, s, operand2, rn, rd)
        {
        }

        protected override void ExecuteInternal(Aarch32CpuState cpuState, uint operand2Value)
        {
            var rn = cpuState.Registers[Rn];
            var rdNewValue = rn & operand2Value;

            cpuState.Registers[Rd] = rdNewValue;
            if (S)
                SetFlagsLogical(cpuState, rdNewValue);
        }

        protected override string ToStringInternal()
        {
            var result = "AND";
            if (S)
                result += "S";
            result += ConditionHelper.ToString(Condition);
            result += " R" + Rd + ", ";
            result += "R" + Rn + ", ";

            return result;
        }
    }
}
