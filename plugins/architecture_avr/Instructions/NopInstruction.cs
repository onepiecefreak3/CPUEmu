using CpuContract;

namespace architecture_avr.Instructions
{
    class NopInstruction : BaseInstruction
    {
        public NopInstruction(int position) : base(position, 2)
        {
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
        }

        public override string ToString()
        {
            return "NOP";
        }
    }
}
