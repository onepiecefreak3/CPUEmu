using System;
using System.Collections.Generic;
using System.Text;
using CpuContract;

namespace assembly_aarch64.Instructions.DataProcessing
{
    // TODO
    class BitfieldDataProcessing : IInstruction
    {
        public int Position { get; }

        public static IInstruction Parse(uint instruction,int position)
        {
            return null;
        }

        public void Execute(IExecutionEnvironment env)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
