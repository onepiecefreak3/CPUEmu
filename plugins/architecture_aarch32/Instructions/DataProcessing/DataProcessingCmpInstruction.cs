using assembly_aarch32.Support;

namespace assembly_aarch32.Instructions.DataProcessing
{
    class DataProcessingCmpInstruction:DataProcessingInstruction
    {
        protected override bool IsLogical => false;

        public DataProcessingCmpInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd) : 
            base(position, condition, i, s, operand2, rn, rd)
        {
        }

        protected override void ExecuteInternal(Aarch32CpuState cpuState,uint operand2Value)
        {
            var rn = cpuState.Registers[Rn];
            var res = rn - operand2Value;

            SetFlagsArithmethic(cpuState, res, rn, operand2Value, false, true);
        }

        protected override string ToStringInternal()
        {
            var result = "CMP";
            result += ConditionHelper.ToString(Condition);
            result += " R" + Rn + ", ";

            return result;
        }
    }
}
