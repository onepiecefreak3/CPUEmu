using CpuContract;

namespace architecture_avr.Instructions.DataTransfer
{
    class InInstruction : BaseInstruction
    {
        private int _rd;
        private int _io;

        public InInstruction(int position, int rd, int io) : base(position, 2)
        {
            _rd = rd;
            _io = io;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            cpuState.Registers[_rd] = env.MemoryMap[0x20 + _io];
        }

        public override string ToString()
        {
            return $"IN R{_rd}, {_io}";
        }
    }
}
