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
        public delegate void LogEventHandler(object sender, string message);

        public abstract event LogEventHandler Log;

        public abstract void Open(Stream input);

        public abstract string Name { get; }

        public abstract int BitVersion { get; }

        public abstract void ExecuteNextInstruction();

        public abstract bool IsFinished { get; }

        public abstract Dictionary<string, long> RetrieveFlags();

        public abstract Dictionary<string, long> RetrieveRegisters();
    }
}
