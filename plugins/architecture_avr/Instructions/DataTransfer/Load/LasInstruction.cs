using CpuContract;

namespace architecture_avr.Instructions.DataTransfer
{
    class LasInstruction : BaseInstruction
    {
        private int _rd;

        public LasInstruction(int position, int rd) : base(position, 2)
        {
            _rd = rd;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var pointer = cpuState.RegZ;

            var value1 = cpuState.Registers[_rd];
            var value2 = env.MemoryMap.ReadByte(pointer);

            var result = (byte) (value1 | value2);
            env.MemoryMap.WriteByte(pointer, result);

            cpuState.Registers[_rd] = result;
        }

        public override string ToString()
        {
            return $"LAS Z, R{_rd}";
        }
    }
}
