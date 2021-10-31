using assembly_aarch32.Support;
using CpuContract;

namespace assembly_aarch32.Instructions.Branch
{
    class BranchAndExchangeInstruction : IExecutableInstruction<Aarch32CpuState>
    {
        private byte _condition;
        private int _rn;

        public bool IsThumbMode => (_rn & 0x1) == 1;

        public int Position { get; }

        public int Length => 4;

        private BranchAndExchangeInstruction(int position, byte condition, int rn)
        {
            Position = position;

            _condition = condition;
            _rn = rn;
        }

        public static BranchAndExchangeInstruction Parse(int position, byte condition, uint instruction)
        {
            var rn = (int)(instruction & 0xF);

            return new BranchAndExchangeInstruction(position, condition, rn);
        }

        public void Execute(Aarch32CpuState cpuState, DeviceEnvironment env)
        {
            if (!ConditionHelper.CanExecute(cpuState, _condition))
                return;

            cpuState.Pc = cpuState.Registers[_rn];
        }

        public override string ToString()
        {
            var result = "BX";
            result += ConditionHelper.ToString(_condition);
            result += $" R{_rn}";
            return result;
        }

        public void Dispose()
        {
            // Nothing to dipose
        }
    }
}
