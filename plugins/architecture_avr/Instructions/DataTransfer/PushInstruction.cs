using CpuContract;

namespace architecture_avr.Instructions.DataTransfer
{
    class PushInstruction : BaseInstruction
    {
        private int _rd;

        public PushInstruction(int position, int rd) : base(position, 2)
        {
            _rd = rd;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            env.MemoryMap.WriteByte(cpuState.Sp, cpuState.Registers[_rd]);
            cpuState.Sp--;
        }

        public override string ToString()
        {
            return $"PUSH R{_rd}";
        }
    }
}
