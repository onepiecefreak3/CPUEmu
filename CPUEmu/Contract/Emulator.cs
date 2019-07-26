using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu.Contract
{
    public enum ByteOrder
    {
        LittleEndian,
        BigEndian
    }

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
        #endregion

        public abstract string Name { get; }

        public abstract int CurrentInstruction { get; }

        public (int, long, string) DisassembleCurrentInstruction() =>
            DisassembleInstructions(CurrentInstruction, 0, 1).First();

        public abstract IEnumerable<(int count, long offset, string source)> DisassembleInstructions(int instrCount, int down, int up);

        public abstract void ExecuteNextInstruction();

        public abstract bool IsFinished { get; }

        public abstract IEnumerable<(string flagName, object value)> GetFlags();

        public abstract IEnumerable<(string registerName, object value)> GetRegisters();

        public abstract void SetFlag(string name, long value);

        public abstract void SetRegister(string name, long value);

        #region Memory operations
        public byte ReadByte(long address) => _mem[address];

        public void WriteByte(long address, byte value) => _mem[address] = value;

        public int ReadInt32(long address, ByteOrder bo = ByteOrder.LittleEndian) =>
            bo == ByteOrder.LittleEndian ? BitConverter.ToInt32(_mem, (int)address) : BitConverter.ToInt32(_mem.Skip((int)address).Take(4).Reverse().ToArray(), 0);

        public void WriteInt32(long address, int value, ByteOrder bo = ByteOrder.LittleEndian)
        {
            if (bo == ByteOrder.LittleEndian)
                Array.Copy(BitConverter.GetBytes(value), 0, _mem, address, 4);
            else
                Array.Copy(BitConverter.GetBytes(value).Reverse().ToArray(), 0, _mem, address, 4);
        }

        public byte[] GetMemoryRange(long address, int count) =>
            _mem.Skip((int)Math.Min(address, _mem.Length)).Take((int)Math.Min(count, _mem.Length - address)).ToArray();

        public void SetMemoryRange(long address, byte[] buffer) =>
            Array.Copy(buffer, 0, _mem, Math.Min(address, _mem.Length), Math.Min(buffer.Length, _mem.Length - address));
        #endregion

        public abstract void Dispose();
    }
}
