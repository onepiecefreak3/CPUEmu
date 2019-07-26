using System;
using CPUEmu.Interfaces;

namespace CPUEmu.AARCH32.Instructions.DataProcessing
{
    class BranchInstruction : IInstruction
    {
        private readonly byte _condition;
        private readonly uint _offset;
        private readonly bool _l;

        public int Position { get; }

        private BranchInstruction(int position, byte condition, uint offset, bool l)
        {
            Position = position;

            _condition = condition;
            _offset = offset;
            _l = l;
        }

        public static IInstruction Parse(int position, byte condition, uint instruction)
        {
            var offset = (instruction & 0xFFFFFF) << 2;

            //sign extend offset
            var sign = (offset >> 25) & 0x1;
            for (int i = 26; i < 32; i++) offset |= sign << i;

            var l = ((instruction >> 24) & 0x1) == 1;

            return new BranchInstruction(position, condition, offset, l);
        }

        public void Execute(ICpuState cpuState)
        {
            if (!ConditionHelper.CanExecute(cpuState, _condition))
                return;

            var pc = Convert.ToUInt32(cpuState.GetRegister("PC"));
            if (_l)
                cpuState.SetRegister("LR", pc - 4);

            cpuState.SetRegister("PC", pc + _offset);
        }

        public override string ToString()
        {
            return $"B{(_l ? "L" : "")}{ConditionHelper.ToString(_condition)} 0x{Position + 8 + _offset:X2}";
        }
    }
}
