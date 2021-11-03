using CpuContract;

namespace architecture_avr.Instructions.Bitwise
{
    class SwapInstruction:BaseInstruction
    {
        private int _rd;

        public SwapInstruction(int position, int rd) : base(position, 2)
        {
            _rd = rd;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var v1 = cpuState.Registers[_rd];
            var n1 = v1 & 0xF;
            var n2 = v1 & 0xF0;

            var result = (n1 << 4) | ((n2 >> 4) & 0xF);
            cpuState.Registers[_rd] = (byte) result;
        }

        public override string ToString()
        {
            return $"SWAP R{_rd}";
        }
    }
}
