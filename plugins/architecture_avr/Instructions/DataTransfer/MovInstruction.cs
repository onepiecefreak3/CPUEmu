using CpuContract;

namespace architecture_avr.Instructions.DataTransfer
{
    class MovInstruction:BaseInstruction
    {
        private int _rd;
        private int _rr;

        public MovInstruction(int position, int rd, int rr) : base(position, 2)
        {
            _rd = rd;
            _rr = rr;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            cpuState.Registers[_rd] = cpuState.Registers[_rr];
        }

        public override string ToString()
        {
            return $"MOV R{_rd}, R{_rr}";
        }
    }
}
