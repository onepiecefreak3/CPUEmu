using CpuContract;

namespace architecture_avr.Instructions.Call
{
    class RcallInstruction : BaseInstruction
    {
        private short _immAdd;

        public RcallInstruction(int position, short immAdd) : base(position, 2)
        {
            _immAdd = immAdd;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            env.MemoryMap.WriteByte(cpuState.Sp--, (byte) (cpuState.Pc + 1));
            env.MemoryMap.WriteByte(cpuState.Sp--, (byte) ((cpuState.Pc + 1) >> 8));

            cpuState.Pc = (uint) (cpuState.Pc + _immAdd + 1);
        }

        public override string ToString()
        {
            return $"RCALL {_immAdd}";
        }
    }
}
