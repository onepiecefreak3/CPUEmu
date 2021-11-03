using CpuContract;

namespace architecture_avr.Instructions.Arithmetic
{
    // HINT: Acts as the SBR instruction, if an immediate is set
    class OrInstruction : BaseInstruction
    {
        private int _rd;
        private int _rr;
        private bool _imm;

        public OrInstruction(int position, int rd, int rr, bool imm = false) : base(position, 2)
        {
            _rd = rd;
            _rr = rr;
            _imm = imm;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var v1 = cpuState.Registers[_rd];
            var v2 = _imm ? (byte)_rr : cpuState.Registers[_rr];
            var res = (byte)(v1 | v2);
            cpuState.Registers[_rd] = res;

            cpuState.V = false;
            cpuState.N = (res & 0x80) == 0x80;
            cpuState.Z = res == 0;
            cpuState.S = cpuState.N ^ cpuState.V;
        }

        public override string ToString()
        {
            return $"OR{(_imm ? "I" : "")} R{_rd}, {(_imm ? "" : "R")}{_rr}";
        }
    }
}
