using CpuContract;

namespace architecture_avr.Instructions.Call
{
    class RetInstruction:BaseInstruction
    {
        public RetInstruction(int position) : base(position, 2)
        {
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var retAdd = 0;
            retAdd |= env.MemoryMap.ReadByte(++cpuState.Sp) << 8;
            retAdd |= env.MemoryMap.ReadByte(++cpuState.Sp);

            cpuState.Pc = (uint)retAdd;
        }

        public override string ToString()
        {
            return "RET";
        }
    }
}
