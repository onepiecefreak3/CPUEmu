using CpuContract;

namespace architecture_avr.Instructions.Call
{
    class CallInstruction:BaseInstruction
    {
        private int _immAdd;

        public CallInstruction(int position, int immAdd) : base(position, 4)
        {
            _immAdd = immAdd;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            env.MemoryMap.WriteByte(cpuState.Sp--, (byte)(cpuState.Pc + 2));
            env.MemoryMap.WriteByte(cpuState.Sp--, (byte)((cpuState.Pc + 2) >> 8));

            cpuState.Pc = (uint)_immAdd;
        }

        public override string ToString()
        {
            return $"CALL {_immAdd}";
        }
    }
}
