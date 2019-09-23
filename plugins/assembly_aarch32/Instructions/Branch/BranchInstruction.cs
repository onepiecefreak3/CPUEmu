using System;
using assembly_aarch32.Support;
using CpuContract;

namespace assembly_aarch32.Instructions.Branch
{
    class BranchInstruction : IInstruction
    {
        private readonly byte _condition;
        private readonly int _offset;
        private readonly bool _l;
        private readonly uint _linkValue;
        private readonly uint _pcValue;

        public bool IsBranching { get; private set; }

        public int Position { get; }

        private BranchInstruction(int position, byte condition, int offset, bool l)
        {
            Position = position;

            _condition = condition;
            _offset = offset;
            _l = l;

            _linkValue = (uint)(position + 4);
            _pcValue = (uint)(position + 8 + offset);
        }

        public static IInstruction Parse(int position, byte condition, uint instruction)
        {
            var offset = (instruction & 0xFFFFFF) << 2;

            // Sign extend offset
            // Offset value contains of 26 bits in total after its shift, and we want to sign extend the 25th (0-indexed)
            var signExtension = 0xFC000000;
            if ((offset & 0x02000000) > 0)
                offset |= signExtension;

            var l = ((instruction >> 24) & 0x1) == 1;

            return new BranchInstruction(position, condition, (int)offset, l);
        }

        public void Execute(IExecutionEnvironment env)
        {
            switch (env.CpuState)
            {
                case Aarch32CpuState armCpuState:
                    if (!ConditionHelper.CanExecute(armCpuState, _condition))
                    {
                        IsBranching = false;
                        return;
                    }

                    if (_l)
                        armCpuState.LR = _linkValue;
                    armCpuState.PC = _pcValue;

                    IsBranching = true;
                    break;
                default:
                    throw new InvalidOperationException("Unknown cpu state.");
            }
        }

        public override string ToString()
        {
            var result = "B";
            if (_l)
                result += "L";
            result += ConditionHelper.ToString(_condition);
            result += $" #{_offset}";
            return result;
        }

        public void Dispose()
        {

        }
    }
}
