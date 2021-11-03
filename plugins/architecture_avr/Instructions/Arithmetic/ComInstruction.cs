using CpuContract;

namespace architecture_avr.Instructions.Arithmetic
{
    class ComInstruction : BaseInstruction
    {
        private int _rd;

        public ComInstruction(int position, int rd) : base(position, 2)
        {
            _rd = rd;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            cpuState.Registers[_rd] = (byte)(0xFF - cpuState.Registers[_rd]);

            cpuState.V = false;
            cpuState.N = (cpuState.Registers[_rd] & 0x80) == 0x80;
            cpuState.Z = cpuState.Registers[_rd] == 0;
            cpuState.C = true;
            cpuState.S = cpuState.N ^ cpuState.V;
        }

        public override string ToString()
        {
            return $"COM R{_rd}";
        }
    }
}
