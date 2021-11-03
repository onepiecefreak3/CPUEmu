using architecture_avr.Support;
using CpuContract;

namespace architecture_avr.Instructions.Branch
{
    class CmpInstruction : BaseInstruction
    {
        private int _rd;
        private int _rr;
        private bool _c;

        public CmpInstruction(int position, int rd, int rr, bool c) : base(position, 2)
        {
            _rd = rd;
            _rr = rr;
            _c = c;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var cv = cpuState.C ? 1 : 0;
            var v1 = cpuState.Registers[_rd];
            var v2 = cpuState.Registers[_rr];
            var res = (byte)(v1 - v2 - (_c ? cv : 0));

            cpuState.H = ((~v1 & 0x4) & (v2 & 0x4) + (v2 & 0x4) & (res & 0x4) + (res & 0x4) & (~v1 & 0x4)) != 0;
            cpuState.N = (res & 0x80) == 0x80;
            cpuState.Z = _c ? res == 0 && cpuState.Z : res == 0;
            cpuState.V = FlagHelper.IsTwoComplementCarry(v1, v2, res, true);
            cpuState.C = v2 + (_c ? cv : 0) > v1;
            cpuState.S = cpuState.N ^ cpuState.V;
        }

        public override string ToString()
        {
            return $"CP{(_c ? "C" : "")} R{_rd}, R{_rr}";
        }
    }
}
