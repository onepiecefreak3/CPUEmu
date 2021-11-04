using CpuContract;

namespace architecture_avr.Instructions.Call
{
    class RetiInstruction : BaseInstruction
    {
        public RetiInstruction(int position) : base(position, 2)
        {
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var retAdd = 0;
            retAdd |= env.MemoryMap.ReadByte((int)++cpuState.Sp) << 8;
            retAdd |= env.MemoryMap.ReadByte((int)++cpuState.Sp);

            cpuState.Pc = (uint)retAdd;
            cpuState.I = true;
        }

        public override string ToString()
        {
            return "RETI";
        }
    }
}
