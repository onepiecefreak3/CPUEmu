using CpuContract;

namespace architecture_avr.Instructions.DataTransfer
{
    class PopInstruction : BaseInstruction
    {
        private int _rd;

        public PopInstruction(int position, int rd) : base(position, 2)
        {
            _rd = rd;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            cpuState.Sp++;
            cpuState.Registers[_rd] = env.MemoryMap.ReadByte(cpuState.Sp);
        }

        public override string ToString()
        {
            return $"POP R{_rd}";
        }
    }
}
