using architecture_avr.Models;
using CpuContract;

namespace architecture_avr.Instructions.Branch
{
    // HINT: This instruction encapsulates all variations of BRBC and BRBS
    class BrInstruction : BaseInstruction
    {
        private sbyte _add;
        private Flag _f;
        private bool _c;

        public bool IsBranching { get; private set; }

        public BrInstruction(int position, sbyte add, Flag flag, bool whenCleared) : base(position, 2)
        {
            _add = add;
            _f = flag;
            _c = whenCleared;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            switch (_f)
            {
                case Flag.Carry:
                    IsBranching = _c && !cpuState.C || !_c && cpuState.C;
                    break;

                case Flag.HalfCarry:
                    IsBranching = _c && !cpuState.H || !_c && cpuState.H;
                    break;

                case Flag.Interrupt:
                    IsBranching = _c && !cpuState.I || !_c && cpuState.I;
                    break;

                case Flag.Negative:
                    IsBranching = _c && !cpuState.N || !_c && cpuState.N;
                    break;

                case Flag.Overflow:
                    IsBranching = _c && !cpuState.V || !_c && cpuState.V;
                    break;

                case Flag.Signed:
                    IsBranching = _c && !cpuState.S || !_c && cpuState.S;
                    break;

                case Flag.T:
                    IsBranching = _c && !cpuState.T || !_c && cpuState.T;
                    break;

                default:
                    IsBranching = _c && !cpuState.Z || !_c && cpuState.Z;
                    break;
            }

            if (IsBranching)
                cpuState.Pc = (uint)(cpuState.Pc + _add + 1);
        }

        public override string ToString()
        {
            switch (_f)
            {
                case Flag.Carry:
                    return $"{(_c ? "BRSH" : "BRLO")} {_add}";

                case Flag.HalfCarry:
                    return $"BRH{(_c ? "C" : "S")} {_add}";

                case Flag.Interrupt:
                    return $"BRI{(_c ? "D" : "E")} {_add}";

                case Flag.Negative:
                    return $"{(_c ? "BRPL" : "BRMI")} {_add}";

                case Flag.Overflow:
                    return $"BRV{(_c ? "C" : "S")} {_add}";

                case Flag.Signed:
                    return $"{(_c ? "BRGE" : "BRLT")} {_add}";

                case Flag.T:
                    return $"BRT{(_c ? "C" : "S")} {_add}";

                default:
                    return $"{(_c ? "BRNE" : "BREQ")} {_add}";
            }
        }
    }
}
