using architecture_avr.Models;
using CpuContract;

namespace architecture_avr.Instructions.Bitwise
{
    class SeInstruction:BaseInstruction
    {
        private Flag _flag;

        public SeInstruction(int position, Flag flag) : base(position, 2)
        {
            _flag = flag;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            switch (_flag)
            {
                case Flag.Carry:
                    cpuState.C = true;
                    break;

                case Flag.HalfCarry:
                    cpuState.H = true;
                    break;

                case Flag.Interrupt:
                    cpuState.I = true;
                    break;

                case Flag.Negative:
                    cpuState.N = true;
                    break;

                case Flag.Overflow:
                    cpuState.V = true;
                    break;

                case Flag.Signed:
                    cpuState.S = true;
                    break;

                case Flag.T:
                    cpuState.T = true;
                    break;

                default:
                    cpuState.Z = true;
                    break;
            }
        }

        public override string ToString()
        {
            switch (_flag)
            {
                case Flag.Carry:
                    return "SEC";

                case Flag.HalfCarry:
                    return "SEH";

                case Flag.Interrupt:
                    return "SEI";

                case Flag.Negative:
                    return "SEN";

                case Flag.Overflow:
                    return "SEV";

                case Flag.Signed:
                    return "SES";

                case Flag.T:
                    return "SET";

                default:
                    return "SEZ";
            }
        }
    }
}
