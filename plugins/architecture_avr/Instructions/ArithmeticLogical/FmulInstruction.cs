using CpuContract;

namespace architecture_avr.Instructions.ArithmeticLogical
{
    class FmulInstruction : BaseInstruction
    {
        private int _rd;
        private int _rr;

        public FmulInstruction(int position, int rd, int rr) : base(position, 2)
        {
            _rd = rd + 16;
            _rr = rr + 16;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var v1 = cpuState.Registers[_rd];
            var v2 = cpuState.Registers[_rr];
            var res = v1 * v2;

            cpuState.C = (res & 0x8000) != 0;

            res <<= 1;
            cpuState.Registers[0] = (byte)res;
            cpuState.Registers[1] = (byte)(res >> 8);

            cpuState.Z = res == 0;
        }

        public override string ToString()
        {
            return $"FMUL R{_rd}, R{_rr}";
        }
    }
}
