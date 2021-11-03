using CpuContract;

namespace architecture_avr.Instructions.Branch
{
    class JmpInstruction:BaseInstruction
    {
        private int _immAdd;

        public JmpInstruction(int position, int immAdd) : base(position, 4)
        {
            _immAdd = immAdd;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            cpuState.Pc = (uint)_immAdd;
        }

        public override string ToString()
        {
            return $"JMP {_immAdd}";
        }
    }
}
