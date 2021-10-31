using CpuContract;

namespace architecture_avr.Instructions.ArithmeticLogical
{
    class AndInstruction : BaseInstruction
    {
        private int _rd;
        private int _rr;
        private bool _imm;

        public AndInstruction(int position, int rd, int rr, bool imm = false) : base(position, 2)
        {
            _rd = rd;
            _rr = rr;
            _imm = imm;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var v1 = cpuState.Registers[_rd];
            var v2 = _imm ? (byte)_rr : cpuState.Registers[_rr];
            var res = (byte)(v1 & v2);
            cpuState.Registers[_rd] = res;

            cpuState.V = false;
            cpuState.N = (res & 0x80) == 0x80;
            cpuState.Z = res == 0;
        }

        public override string ToString()
        {
            return $"AND{(_imm ? "I" : "")} R{_rd}, {(_imm ? "" : "R")}{_rr}";
        }
    }
}
