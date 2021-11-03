using architecture_avr.Support;
using CpuContract;

namespace architecture_avr.Instructions.Arithmetic
{
    class AddInstruction : BaseInstruction
    {
        private int _rd;
        private int _rr;
        private bool _c;

        public AddInstruction(int position, int rd, int rr, bool c) : base(position, 2)
        {
            _rd = rd;
            _rr = rr;
            _c = c;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var v1 = cpuState.Registers[_rd];
            var v2 = cpuState.Registers[_rr];
            var cv = _c ? 1 : 0;
            var res = (byte)(v1 + v2 + (_c ? cv : 0));

            cpuState.Registers[_rd] = res;

            cpuState.H = ((v1 & 0x4) & (v2 & 0x4) + (v2 & 0x4) & (~res & 0x4) + (~res & 0x4) & (v1 & 0x4)) != 0;
            cpuState.N = (res & 0x80) == 0x80;
            cpuState.Z = res == 0;
            cpuState.V = FlagHelper.IsTwoComplementCarry(v1, v2, res, true);
            cpuState.C = _c ? res <= v1 : res < v1;
            cpuState.S = cpuState.N ^ cpuState.V;
        }

        public override string ToString()
        {
            return $"AD{(_c ? "C" : "D")} R{_rd}, R{_rr}";
        }
    }
}
