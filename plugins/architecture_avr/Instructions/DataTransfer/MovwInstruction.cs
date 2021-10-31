using CpuContract;

namespace architecture_avr.Instructions.DataTransfer
{
    class MovwInstruction : BaseInstruction
    {
        private int _rd;
        private int _rr;

        public MovwInstruction(int position, int rd, int rr) : base(position, 2)
        {
            _rd = rd;
            _rr = rr;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            cpuState.Registers[_rd * 2] = cpuState.Registers[_rr * 2];
            cpuState.Registers[_rd * 2 + 1] = cpuState.Registers[_rr * 2 + 1];
        }

        public override string ToString()
        {
            return $"MOVW R{_rd}, R{_rr}";
        }
    }
}
