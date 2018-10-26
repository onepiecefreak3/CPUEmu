using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu.Contract
{
    public abstract class Emulator : IDisposable
    {
        protected byte[] _mem;

        protected Emulator(byte[] payload, long payloadOffset, long stackOffset, int stackSize)
        {
            var maxSize = Math.Max(payloadOffset + payload.Length, stackOffset + stackSize);
            if (maxSize > int.MaxValue)
                throw new InsufficientMemoryException();

            _mem = new byte[maxSize];
            Array.Copy(payload, 0, _mem, payloadOffset, payload.Length);

            PayloadOffset = payloadOffset;
            PayloadLength = payload.Length;
        }

        public long PayloadOffset { get; }

        public long PayloadLength { get; }

        #region Events
        public delegate void LogEventHandler(object sender, string message);

        public abstract event LogEventHandler Log;

        //public delegate void DisassembleEventHandler(object sender, long address, string source);

        //public abstract event DisassembleEventHandler Disassemble;
        #endregion

        public abstract string Name { get; }

        public abstract int BitVersion { get; }

        public abstract int BitsPerInstruction { get; }

        public abstract long CurrentInstructionOffset { get; }

        public (long, string) DisassembleCurrentInstruction()
        {
            return DisassembleInstructions(CurrentInstructionOffset, 1).First();
        }

        public abstract IEnumerable<(long offset, string source)> DisassembleInstructions(long offset, int count);

        public abstract void ExecuteNextInstruction();

        public abstract bool IsFinished { get; }

        public abstract IEnumerable<(string flagName, long value)> RetrieveFlags();

        public abstract IEnumerable<(string registerName, long value)> RetrieveRegisters();

        public abstract void SetFlag(string name, long value);

        public abstract void SetRegister(string name, long value);

        #region Get/Set memory
        public byte GetMemoryByte(long address)
        {
            return _mem[address];
        }

        public void SetMemoryByte(long address, byte value)
        {
            _mem[address] = value;
        }

        public byte[] GetMemoryRange(long address, int count)
        {
            count = (int)Math.Min(count, _mem.Length - address);
            var buffer = new byte[count];
            Array.Copy(_mem, address, buffer, 0, count);
            return buffer;
        }

        public void SetMemoryRange(long address, byte[] buffer)
        {
            Array.Copy(buffer, 0, _mem, address, buffer.Length);
        }
        #endregion

        public abstract void Dispose();
    }
}
