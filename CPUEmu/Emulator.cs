using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu
{
    public abstract class Emulator
    {
        protected BinaryReader _binary;

        protected Emulator(Stream input)
        {
            _binary = new BinaryReader(input);
        }

        public delegate void LogEventHandler(object sender, string message);

        public abstract event LogEventHandler Log;

        public delegate void PrintEventHandler(object sender, string print);

        public abstract event PrintEventHandler Print;

        public abstract string Name { get; }

        public abstract int BitVersion { get; }

        public abstract int BitsPerInstruction { get; }

        public abstract long CurrentInstructionOffset { get; }

        public void PrintCurrentInstruction()
        {
            PrintInstructions(CurrentInstructionOffset, 1);
        }

        public abstract void PrintInstructions(long offset, int count);

        public abstract void ExecuteNextInstruction();

        public abstract bool IsFinished { get; }

        public abstract Dictionary<string, long> RetrieveFlags();

        public abstract Dictionary<string, long> RetrieveRegisters();
    }
}
