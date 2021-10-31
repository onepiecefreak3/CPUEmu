using architecture_avr.Support;
using CpuContract;

namespace architecture_avr.Instructions.ArithmeticLogical
{
    class CpiInstruction : BaseInstruction
    {
        private int _rd;
        private byte _imm;

        public CpiInstruction(int position, int rd, int imm) : base(position, 2)
        {
            _rd = rd + 16;
            _imm = (byte)imm;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var rn = cpuState.Registers[_rd];
            var res = (byte)(rn - _imm);

            cpuState.H = ((~rn & 0x4) & (_imm & 0x4) + (_imm & 0x4) & (res & 0x4) + (res & 0x4) & (~rn & 0x4)) != 0;
            cpuState.N = (res & 0x80) == 0x80;
            cpuState.Z = res == 0;
            cpuState.V = FlagHelper.IsTwoComplementCarry(rn, _imm, res, false);
            cpuState.C = _imm > rn;
        }

        public override string ToString()
        {
            return $"CPI R{_rd}, {_imm}";
        }
    }
}
