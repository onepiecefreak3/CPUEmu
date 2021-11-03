using System;
using CpuContract;

namespace architecture_avr.Instructions.Branch
{
    class IjmpInstructioncs : BaseInstruction
    {
        private bool _ext;

        public IjmpInstructioncs(int position, bool ext) : base(position, 2)
        {
            _ext = ext;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            if(_ext)
                throw new InvalidOperationException("Unsupported extended JMP/CALL");

            cpuState.Pc = cpuState.RegZ;
        }

        public override string ToString()
        {
            return $"{(_ext ? "E" : "")}IJMP";
        }
    }
}
