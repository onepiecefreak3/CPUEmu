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

        public bool IsBranching { get; private set; }

        public int Position { get; }

        private BranchInstruction(int position, byte condition, int offset, bool l)
        {
            Position = position;

            _condition = condition;
            _offset = offset;
            _l = l;
        }

        public static IInstruction Parse(int position, byte condition, uint instruction)
        {
            var offset = (int)((instruction & 0xFFFFFF) << 2);

            // Sign extend offset
            // Offset value contains of 26 bits in total after its shift, and we want to sign extend the 25th (0-indexed)
            var sign = offset >> 25;
            for (int i = 26; i < 32; i++)
                offset |= sign << i;

            var l = ((instruction >> 24) & 0x1) == 1;

            return new BranchInstruction(position, condition, offset, l);
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

                    var pc = armCpuState.PC;
                    if (_l)
                        armCpuState.LR = pc - 4;

                    armCpuState.PC = (uint)(pc + _offset);
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
