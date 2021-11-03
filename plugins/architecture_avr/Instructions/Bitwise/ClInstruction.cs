using architecture_avr.Models;
using CpuContract;

namespace architecture_avr.Instructions.Bitwise
{
    class ClInstruction : BaseInstruction
    {
        private Flag _flag;

        public ClInstruction(int position, Flag flag) : base(position, 2)
        {
            _flag = flag;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            switch (_flag)
            {
                case Flag.Carry:
                    cpuState.C = false;
                    break;

                case Flag.HalfCarry:
                    cpuState.H = false;
                    break;

                case Flag.Interrupt:
                    cpuState.I = false;
                    break;

                case Flag.Negative:
                    cpuState.N = false;
                    break;

                case Flag.Overflow:
                    cpuState.V = false;
                    break;

                case Flag.Signed:
                    cpuState.S = false;
                    break;

                case Flag.T:
                    cpuState.T = false;
                    break;

                default:
                    cpuState.Z = false;
                    break;
            }
        }

        public override string ToString()
        {
            switch (_flag)
            {
                case Flag.Carry:
                    return "CLC";

                case Flag.HalfCarry:
                    return "CLH";

                case Flag.Interrupt:
                    return "CLI";

                case Flag.Negative:
                    return "CLN";

                case Flag.Overflow:
                    return "CLV";

                case Flag.Signed:
                    return "CLS";

                case Flag.T:
                    return "CLT";

                default:
                    return "CLZ";
            }
        }
    }
}
