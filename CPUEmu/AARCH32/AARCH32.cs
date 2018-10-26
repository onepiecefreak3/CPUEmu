using CPUEmu.Contract;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//http://vision.gel.ulaval.ca/~jflalonde/cours/1001/h17/docs/arm-instructionset.pdf

namespace CPUEmu
{
    public partial class AARCH32 : Emulator
    {
        #region Emulator specific
        private bool _z;
        private bool _c;
        private bool _n;
        private bool _v;

        private uint[] _reg = new uint[16];

        private uint _sp { get => _reg[13]; set => _reg[13] = value; }
        private uint _lr { get => _reg[14]; set => _reg[14] = value; }
        private uint _pc { get => _reg[15]; set => _reg[15] = value; }

        Queue<uint> _instrBuffer = new Queue<uint>();
        #endregion

        #region Disassembling related
        private string _currentCondition;
        private long _currentInstrOffset;
        #endregion

        #region Abstracts
        public AARCH32(byte[] payload, long payloadOffset, long stackOffset, int stackSize) : base(payload, payloadOffset, stackOffset, stackSize)
        {
            _pc = (uint)payloadOffset;
            _sp = (uint)stackOffset;

            //Buffer next 2 instructions
            ReadNextInstruction();
            ReadNextInstruction();
        }

        public override string Name => "AARCH32";

        public override int BitVersion => 32;

        public override int BitsPerInstruction => 32;

        public override long CurrentInstructionOffset => _pc - 8;

        public override bool IsFinished => !_instrBuffer.Any();

        public override event LogEventHandler Log;

        public override void ExecuteNextInstruction()
        {
            var instruction = _instrBuffer.Dequeue();

            if (CheckConditions((byte)(instruction >> 28)))
                ExecuteInstructionType(instruction);

            ReadNextInstruction();
        }

        public override IEnumerable<(long, string)> DisassembleInstructions(long offset, int count)
        {
            for (var i = offset; i < offset + count * 4; i += 4)
            {
                if (i >= PayloadOffset && i < PayloadOffset + PayloadLength)
                {
                    _currentInstrOffset = i;

                    var instruction = ReadUInt32(i);
                    DisassembleConditions((byte)(instruction >> 28));
                    yield return (_currentInstrOffset, DisassembleInstructionType(instruction));
                }
            }
        }

        public override IEnumerable<(string flagName, long value)> RetrieveFlags()
        {
            return new List<(string flagName, long value)>
            {
                ("Z", _z ? 1 : 0),
                ("C", _c ? 1 : 0),
                ("N", _n ? 1 : 0),
                ("V", _v ? 1 : 0)
            };
        }

        public override IEnumerable<(string registerName, long value)> RetrieveRegisters()
        {
            return new List<(string registerName, long value)>
            {
                ("R0", _reg[0]),
                ("R1", _reg[1]),
                ("R2", _reg[2]),
                ("R3", _reg[3]),
                ("R4", _reg[4]),
                ("R5", _reg[5]),
                ("R6", _reg[6]),
                ("R7", _reg[7]),
                ("R8", _reg[8]),
                ("R9", _reg[9]),
                ("R10", _reg[10]),
                ("R11", _reg[11]),
                ("R12", _reg[12]),
                ("SP", _reg[13]),
                ("LR", _reg[14]),
                ("PC", _reg[15])
            };
        }

        public override void SetFlag(string name, long value)
        {
            switch (name)
            {
                case "Z":
                    _z = value != 0;
                    break;
                case "C":
                    _c = value != 0;
                    break;
                case "N":
                    _n = value != 0;
                    break;
                case "V":
                    _v = value != 0;
                    break;
                default:
                    throw new InvalidDataException(name);
            }
        }

        public override void SetRegister(string name, long value)
        {
            switch (name)
            {
                case "R0":
                    _reg[0] = (uint)value;
                    break;
                case "R1":
                    _reg[1] = (uint)value;
                    break;
                case "R2":
                    _reg[2] = (uint)value;
                    break;
                case "R3":
                    _reg[3] = (uint)value;
                    break;
                case "R4":
                    _reg[4] = (uint)value;
                    break;
                case "R5":
                    _reg[5] = (uint)value;
                    break;
                case "R6":
                    _reg[6] = (uint)value;
                    break;
                case "R7":
                    _reg[7] = (uint)value;
                    break;
                case "R8":
                    _reg[8] = (uint)value;
                    break;
                case "R9":
                    _reg[9] = (uint)value;
                    break;
                case "R10":
                    _reg[10] = (uint)value;
                    break;
                case "R11":
                    _reg[11] = (uint)value;
                    break;
                case "R12":
                    _reg[12] = (uint)value;
                    break;
                case "SP":
                case "R13":
                    _reg[13] = (uint)value;
                    break;
                case "LR":
                case "R14":
                    _reg[14] = (uint)value;
                    break;
                case "PC":
                case "R15":
                    SetPC((uint)value);
                    break;
                default:
                    throw new InvalidDataException(name);
            }
        }
        #endregion

        #region Helper
        private uint ReadUInt32(long offset) => (uint)(_mem[offset] | (_mem[offset + 1] << 8) | (_mem[offset + 2] << 16) | (_mem[offset + 3] << 24));
        private void WriteUInt32(long offset, uint value)
        {
            _mem[offset] = (byte)value;
            _mem[offset + 1] = (byte)(value >> 8);
            _mem[offset + 2] = (byte)(value >> 16);
            _mem[offset + 3] = (byte)(value >> 24);
        }
        #endregion

        private void ReadNextInstruction()
        {
            if (_pc >= PayloadOffset && _pc < PayloadOffset + PayloadLength)
                _instrBuffer.Enqueue(ReadUInt32(_pc));
            _pc += 4;
        }

        private void SetPC(uint offset)
        {
            _pc = offset;

            _instrBuffer.Clear();

            ReadNextInstruction();
            ReadNextInstruction();
        }

        private void UpdateFlags(bool z, bool c, bool n, bool v)
        {
            _z = z;
            _c = c;
            _n = n;
            _v = v;
        }
    }
}
