using CpuContract;

namespace architecture_avr.Instructions.DataTransfer.Load
{
    class LdsInstruction : BaseInstruction
    {
        private int _rd;
        private int _address;

        public LdsInstruction(int position, int rd, int address) : base(position, 4)
        {
            _rd = rd;
            _address = address;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            cpuState.Registers[_rd] = env.MemoryMap.ReadByte(_address);
        }

        public override string ToString()
        {
            return $"LDS R{_rd}, {_address}";
        }
    }
}
