using CpuContract;

namespace architecture_avr.Instructions.Branch
{
    class CpseInstruction : BaseInstruction
    {
        private int _rd;
        private int _rr;

        public bool IsSkipping { get; private set; }

        public CpseInstruction(int position, int rd, int rr) : base(position, 2)
        {
            _rd = rd;
            _rr = rr;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            IsSkipping = cpuState.Registers[_rd] == cpuState.Registers[_rr];
        }
    }
}
