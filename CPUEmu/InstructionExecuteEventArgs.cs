using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPUEmu.Interfaces;

namespace CPUEmu
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
