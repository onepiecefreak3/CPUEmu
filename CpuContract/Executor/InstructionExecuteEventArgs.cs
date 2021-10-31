using System;

namespace CpuContract.Executor
{
    public class InstructionExecuteEventArgs:EventArgs
    {
        public IInstruction Instruction { get; }
        public int Index { get; }

        public InstructionExecuteEventArgs(IInstruction instruction, int index)
        {
            Instruction = instruction;
            Index = index;
        }
    }
}
