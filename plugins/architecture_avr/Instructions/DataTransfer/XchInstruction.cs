using CpuContract;

namespace architecture_avr.Instructions.DataTransfer
{
    class XchInstruction : BaseInstruction
    {
        private int _rd;

        public XchInstruction(int position, int rd) : base(position, 2)
        {
            _rd = rd;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var pointer = cpuState.RegZ;

            var value1 = cpuState.Registers[_rd];
            var value2 = env.MemoryMap.ReadByte(pointer);

            cpuState.Registers[_rd] = value2;
            env.MemoryMap.WriteByte(pointer, value1);
        }

        public override string ToString()
        {
            return $"XCH Z, R{_rd}";
        }
    }
}
