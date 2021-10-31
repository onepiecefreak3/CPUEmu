using assembly_aarch32.Support;
using CpuContract;

namespace assembly_aarch32.Instructions.Branch
{
    class BranchInstruction : IExecutableInstruction<Aarch32CpuState>
    {
        private readonly byte _condition;
        private readonly int _offset;
        private readonly bool _l;

        private readonly uint _linkValue;
        private readonly uint _pcValue;

        public bool IsBranching { get; private set; }

        public int Position { get; }

        public int Length => 4;

        private BranchInstruction(int position, byte condition, int offset, bool l)
        {
            Position = position;

            _condition = condition;
            _offset = offset;
            _l = l;

            _linkValue = (uint)(position + 4);
            _pcValue = (uint)(position + offset + 8);
        }

        public static BranchInstruction Parse(int position, byte condition, uint instruction)
        {
            var offset = (instruction & 0xFFFFFF) << 2;

            // Sign extend offset
            // Offset value contains of 26 bits in total after its shift, and we want to sign extend the 25th (0-indexed)
            var signExtend = 0xFC000000;
            if ((offset & 0x02000000) != 0)
                offset |= signExtend;
            var l = ((instruction >> 24) & 0x1) == 1;

            return new BranchInstruction(position, condition, (int)offset, l);
        }

        public void Execute(Aarch32CpuState cpuState, DeviceEnvironment env)
        {
            if (!ConditionHelper.CanExecute(cpuState, _condition))
            {
                IsBranching = false;
                return;
            }

            if (_l)
                cpuState.Lr = _linkValue;
            cpuState.Pc = _pcValue;

            IsBranching = true;
        }

        public override string ToString()
        {
            var result = "B";
            if (_l)
                result += "L";
            result += ConditionHelper.ToString(_condition);

            result += $" #{_offset + 8}";
            return result;
        }

        public void Dispose()
        {

        }
    }
}
