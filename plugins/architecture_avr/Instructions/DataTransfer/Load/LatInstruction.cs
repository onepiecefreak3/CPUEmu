using CpuContract;

namespace architecture_avr.Instructions.DataTransfer.Load
{
    class LatInstruction:BaseInstruction
    {
        private int _rd;

        public LatInstruction(int position, int rd) : base(position, 2)
        {
            _rd = rd;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var pointer = cpuState.RegZ;

            var value1 = env.MemoryMap.ReadByte(pointer);
            var value2 = cpuState.Registers[_rd];

            var result = (byte)(value1 ^ value2);
            env.MemoryMap.WriteByte(pointer,result);

            cpuState.Registers[_rd] = result;
        }

        public override string ToString()
        {
            return $"LAT Z, R{_rd}";
        }
    }
}
