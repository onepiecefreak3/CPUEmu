using CpuContract;

namespace architecture_avr.Instructions.DataTransfer.Store
{
    class StsInstruction : BaseInstruction
    {
        private int _rd;
        private int _address;

        public StsInstruction(int position, int rd, int address) : base(position, 4)
        {
            _rd = rd;
            _address = address;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            env.MemoryMap.WriteByte(_address, cpuState.Registers[_rd]);
        }

        public override string ToString()
        {
            return $"STS {_address}, R{_rd}";
        }
    }
}
