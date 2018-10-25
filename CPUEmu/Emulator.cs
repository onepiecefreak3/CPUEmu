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
        protected byte[] _mem;

        protected Emulator(byte[] input, int binaryEntry, int stackAddress, int stackSize)
        {
            var maxSize = Math.Max(binaryEntry + input.Length, stackAddress + stackSize);
            if (maxSize > Int32.MaxValue)
                throw new InsufficientMemoryException();

            _mem = new byte[maxSize];
            Array.Copy(input, 0, _mem, binaryEntry, input.Length);
        }

        public delegate void LogEventHandler(object sender, string message);

        public abstract event LogEventHandler Log;

        public delegate void DisassembleEventHandler(object sender, long address, string source);

        public abstract event DisassembleEventHandler Disassemble;

        public abstract string Name { get; }

        public abstract int BitVersion { get; }

        public abstract int BitsPerInstruction { get; }

        public abstract long CurrentInstructionOffset { get; }

        public void DisassembleCurrentInstruction()
        {
            DisassembleInstructions(CurrentInstructionOffset, 1);
        }

        public abstract void DisassembleInstructions(long offset, int count);

        public abstract void ExecuteNextInstruction();

        public abstract bool IsFinished { get; }

        public abstract List<(string flagName, long value)> RetrieveFlags();

        public abstract List<(string registerName, long value)> RetrieveRegisters();

        public void SetMemoryByte(long address, byte value)
        {
            _mem[address] = value;
        }

        public byte GetMemoryByte(long address)
        {
            return _mem[address];
        }

        public byte[] GetMemoryRange(long address, int count)
        {
            count = (int)Math.Min(count, _mem.Length - address);
            var buffer = new byte[count];
            Array.Copy(_mem, address, buffer, 0, count);
            return buffer;
        }
    }
}
