using System;
using CpuContract;

namespace architecture_avr.Instructions.DataTransfer
{
    class LdInstruction : BaseInstruction
    {
        public LdInstruction(int position, int rd, int pointer, bool preDec = false, bool postInc = false) : base(position, 2)
        {
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            throw new NotImplementedException();
        }
    }
}
