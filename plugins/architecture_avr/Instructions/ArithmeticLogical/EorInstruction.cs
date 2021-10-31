using CpuContract;

namespace architecture_avr.Instructions.ArithmeticLogical
{
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
        }

        public override string ToString()
        {
            return $"EOR R{_rd}, R{_rr}";
        }
    }
}
