using CpuContract;

namespace architecture_avr.Instructions.Arithmetic
{
    class NegInstruction : BaseInstruction
    {
        private int _rd;

        public NegInstruction(int position, int rd) : base(position, 2)
        {
            _rd = rd;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var v1 = cpuState.Registers[_rd];
            var result = (byte)(0 - v1);
            cpuState.Registers[_rd] = result;

            cpuState.H = (result & 0x4) + (~v1 & 0x4) != 0;
            cpuState.V = result == 0x80;
            cpuState.N = (v1 & 0x80) == 0x80;
            cpuState.Z = v1 == 0;
            cpuState.C = result != 0;
            cpuState.S = cpuState.N ^ cpuState.V;
        }

        public override string ToString()
        {
            return $"NEG R{_rd}";
        }
    }
}
