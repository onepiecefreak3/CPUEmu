using CpuContract;

namespace architecture_avr.Instructions.Arithmetic
{
    // HINT: Acts as CLR instruction if rd and rr are equal
    class EorInstruction : BaseInstruction
    {
        private int _rd;
        private int _rr;

        public EorInstruction(int position, int rd, int rr) : base(position, 2)
        {
            _rd = rd;
            _rr = rr;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var res = (byte)(cpuState.Registers[_rd] ^ cpuState.Registers[_rr]);
            cpuState.Registers[_rd] = res;

            cpuState.V = false;
            cpuState.N = (res & 0x80) == 0x80;
            cpuState.Z = res == 0;
            cpuState.S = cpuState.N ^ cpuState.V;
        }

        public override string ToString()
        {
            return _rd == _rr ? $"CLR R{_rd}" : $"EOR R{_rd}, R{_rr}";
        }
    }
}
