using System;
using CpuContract;

namespace architecture_avr.Instructions.Call
{
    class IcallInstruction : BaseInstruction
    {
        private bool _ext;

        public IcallInstruction(int position, bool ext) : base(position, 2)
        {
            _ext = ext;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            if (_ext)
                throw new InvalidOperationException("Unsupported extended JMP/CALL");

            env.MemoryMap.WriteByte((int)cpuState.Sp--, (byte)(cpuState.Pc + 1));
            env.MemoryMap.WriteByte((int)cpuState.Sp--, (byte)((cpuState.Pc + 1) >> 8));

            cpuState.Pc = cpuState.RegZ;
        }

        public override string ToString()
        {
            return $"{(_ext ? "E" : "")}ICALL";
        }
    }
}
