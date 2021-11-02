using architecture_avr.Models;
using CpuContract;

namespace architecture_avr.Instructions.DataTransfer.Load
{
    class LdInstruction : BaseInstruction
    {
        private int _rd;
        private Pointer _p;
        private PointerModification _pm;

        public LdInstruction(int position, int rd, Pointer pointer, PointerModification mod) : base(position, 2)
        {
            _rd = rd;
            _p = pointer;
            _pm = mod;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            ushort pointer = 0;
            switch (_p)
            {
                case Pointer.X:
                    pointer = cpuState.RegX;
                    break;

                case Pointer.Y:
                    pointer = cpuState.RegY;
                    break;

                case Pointer.Z:
                    pointer = cpuState.RegZ;
                    break;
            }

            if (_pm == PointerModification.PreDecrement)
                pointer--;

            cpuState.Registers[_rd] = env.MemoryMap.ReadByte(pointer);

            if (_pm == PointerModification.PostIncrement)
                pointer++;

            switch (_p)
            {
                case Pointer.X:
                    cpuState.RegX = pointer;
                    break;

                case Pointer.Y:
                    cpuState.RegY = pointer;
                    break;

                case Pointer.Z:
                    cpuState.RegZ = pointer;
                    break;
            }
        }

        public override string ToString()
        {
            return $"LD R{_rd}, {((byte)_pm == 2 ? "-" : "")}{_p}{((byte)_pm == 1 ? "+" : "")}";
        }
    }
}
