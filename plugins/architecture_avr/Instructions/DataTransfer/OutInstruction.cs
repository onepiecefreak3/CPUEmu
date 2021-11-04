using CpuContract;

namespace architecture_avr.Instructions.DataTransfer
{
    class OutInstruction : BaseInstruction
    {
        private int _rd;
        private int _io;

        public OutInstruction(int position, int rd, int io) : base(position, 2)
        {
            _rd = rd;
            _io = io;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            env.MemoryMap[0x20 + _io] = cpuState.Registers[_rd];
        }

        public override string ToString()
        {
            return $"OUT {_io}, R{_rd}";
        }
    }
}
