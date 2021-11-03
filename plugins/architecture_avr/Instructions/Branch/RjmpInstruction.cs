using CpuContract;

namespace architecture_avr.Instructions.Branch
{
    class RjmpInstruction : BaseInstruction
    {
        private short _immAdd;

        public RjmpInstruction(int position, short immAdd) : base(position, 2)
        {
            _immAdd = immAdd;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            cpuState.Pc = (uint)(cpuState.Pc + _immAdd + 1);
        }

        public override string ToString()
        {
            return $"RJMP {_immAdd}";
        }
    }
}
