using CpuContract;

namespace architecture_avr.Instructions.Bitwise
{
    class LsrInstruction : BaseInstruction
    {
        private int _rd;

        public LsrInstruction(int position, int rd) : base(position, 2)
        {
            _rd = rd;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var v1 = cpuState.Registers[_rd];
            var res = v1 >> 1;

            cpuState.Registers[_rd] = (byte)res;

            cpuState.N = false;
            cpuState.Z = res == 0;
            cpuState.C = (v1 & 0x1) == 1;
            cpuState.V = cpuState.N ^ cpuState.C;
            cpuState.S = cpuState.N ^ cpuState.V;
        }
    }
}
