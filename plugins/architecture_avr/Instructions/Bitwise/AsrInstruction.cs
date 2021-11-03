using CpuContract;

namespace architecture_avr.Instructions.Bitwise
{
    class AsrInstruction : BaseInstruction
    {
        private int _rd;

        public AsrInstruction(int position, int rd) : base(position, 2)
        {
            _rd = rd;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var v1 = cpuState.Registers[_rd];
            var res = (v1 & 0x80) | (v1 >> 1);

            cpuState.Registers[_rd] = (byte)res;

            cpuState.N = (res & 0x80) == 0x80;
            cpuState.Z = res == 0;
            cpuState.C = (v1 & 0x1) == 1;
            cpuState.V = cpuState.N ^ cpuState.C;
            cpuState.S = cpuState.N ^ cpuState.V;
        }

        public override string ToString()
        {
            return $"ASR R{_rd}";
        }
    }
}
