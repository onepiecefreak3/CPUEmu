using CpuContract;

namespace architecture_avr.Instructions.Arithmetic
{
    class DecInstruction : BaseInstruction
    {
        private int _rd;

        public DecInstruction(int position, int rd) : base(position, 2)
        {
            _rd=rd;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var v1 = cpuState.Registers[_rd];
            var res = v1 - 1;

            cpuState.Registers[_rd] = (byte)res;

            cpuState.V = v1 == 0x80;
            cpuState.N = (res & 0x80) == 0x80;
            cpuState.Z = res == 0;
            cpuState.S = cpuState.N ^ cpuState.V;
        }

        public override string ToString()
        {
            return $"DEC R{_rd}";
        }
    }
}
