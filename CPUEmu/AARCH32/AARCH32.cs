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
        private bool _z;
        private bool _c;
        private bool _n;
        private bool _v;

        private uint[] _reg = new uint[16];

        private uint _sp { get => _reg[13]; set => _reg[13] = value; }
        private uint _lr { get => _reg[14]; set => _reg[14] = value; }
        private uint _pc { get => _reg[15]; set => _reg[15] = value; }

        Queue<uint> _instrBuffer = new Queue<uint>();

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

        #region Data Processing
        private void DataProcessing(uint instruction)
        {
            var op2 = instruction & 0xFFF;
            var rd = (instruction >> 12) & 0xF;
            var rn = (instruction >> 16) & 0xF;
            var s = (instruction >> 20) & 0x1;
            var opcode = (instruction >> 21) & 0xF;
            var i = (instruction >> 25) & 0x1;

            var op2v = GetOp2Value(i == 1, op2, s == 1);

            switch (opcode)
            {
                //AND
                case 0:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"AND{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        _reg[rd] = _reg[rn] & op2v;
                        if (s == 1)
                            UpdateFlags(_reg[rd] == 0, _c, (_reg[rd] >> 31) == 1, _v);
                    }
                    break;

                //EOR
                case 1:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"EOR{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        _reg[rd] = _reg[rn] ^ op2v;
                        if (s == 1)
                            UpdateFlags(_reg[rd] == 0, _c, (_reg[rd] >> 31) == 1, _v);
                    }
                    break;

                //SUB
                case 2:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"SUB{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        _reg[rd] = _reg[rn] - op2v;
                        if (s == 1)
                            UpdateFlags(_reg[rd] == 0, _reg[rd] < _reg[rn], (_reg[rd] >> 31) == 1, (_reg[rd] >> 31) != (_reg[rn] >> 31));
                    }
                    break;

                //RSB
                case 3:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"RSB{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}, R{rn}");
                    else
                    {
                        _reg[rd] = op2v - _reg[rn];
                        if (s == 1)
                            UpdateFlags(_reg[rd] == 0, _reg[rd] < op2v, (_reg[rd] >> 31) == 1, (_reg[rd] >> 31) != (op2v >> 31));
                    }
                    break;

                //ADD
                case 4:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"ADD{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        _reg[rd] = _reg[rn] + op2v;
                        if (s == 1)
                            UpdateFlags(_reg[rd] == 0, _reg[rd] < _reg[rn], (_reg[rd] >> 31) == 1, (_reg[rd] >> 31) != (_reg[rn] >> 31));
                    }
                    break;

                //ADC
                case 5:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"ADC{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        _reg[rd] = _reg[rn] + op2v;
                        if (s == 1)
                            UpdateFlags(_reg[rd] == 0, _reg[rd] < _reg[rn], (_reg[rd] >> 31) == 1, (_reg[rd] >> 31) != (_reg[rn] >> 31));
                        _reg[rd] += (uint)(_c ? 1 : 0);
                    }
                    break;

                //SBC
                case 6:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"SBC{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        _reg[rd] = _reg[rn] - op2v;
                        if (s == 1)
                            UpdateFlags(_reg[rd] == 0, _reg[rd] < _reg[rn], (_reg[rd] >> 31) == 1, (_reg[rd] >> 31) != (_reg[rn] >> 31));
                        _reg[rd] += (uint)(_c ? 0 : -1);
                    }
                    break;

                //RSC
                case 7:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"RSC{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}, R{rn}");
                    else
                    {
                        _reg[rd] = op2v - _reg[rn];
                        if (s == 1)
                            UpdateFlags(_reg[rd] == 0, _reg[rd] < _reg[rn], (_reg[rd] >> 31) == 1, (_reg[rd] >> 31) != (_reg[rn] >> 31));
                        _reg[rd] += (uint)(_c ? 0 : -1);
                    }
                    break;

                //TST
                case 8:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"TST{_currentCondition} R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        var res = _reg[rn] & op2v;
                        UpdateFlags(res == 0, _c, (res >> 31) == 1, _v);
                    }
                    break;

                //TEQ
                case 9:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"TEQ{_currentCondition} R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        var res = _reg[rn] ^ op2v;
                        UpdateFlags(res == 0, _c, (res >> 31) == 1, _v);
                    }
                    break;

                //CMP
                case 10:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"CMP{_currentCondition} R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        var res = _reg[rn] - op2v;
                        UpdateFlags(res == 0, res < _reg[rn], (res >> 31) == 1, (res >> 31) != (_reg[rn] >> 31));
                    }
                    break;

                //CMN
                case 11:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"CMN{_currentCondition} R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        var res = _reg[rn] + op2v;
                        UpdateFlags(res == 0, res < _reg[rn], (res >> 31) == 1, (res >> 31) != (_reg[rn] >> 31));
                    }
                    break;

                //ORR
                case 12:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"ORR{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        _reg[rd] = _reg[rn] | op2v;
                        if (s == 1)
                            UpdateFlags(_reg[rd] == 0, _c, (_reg[rd] >> 31) == 1, _v);
                    }
                    break;

                //MOV
                case 13:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"MOV{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        _reg[rd] = op2v;
                        if (s == 1)
                            UpdateFlags(_reg[rd] == 0, _c, (_reg[rd] >> 31) == 1, _v);
                    }
                    break;

                //BIC
                case 14:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"BIC{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        _reg[rd] = _reg[rn] & ~op2v;
                        if (s == 1)
                            UpdateFlags(_reg[rd] == 0, _c, (_reg[rd] >> 31) == 1, _v);
                    }
                    break;

                //MVN
                case 15:
                    if (_currentlyPrinting)
                        Disassemble(_currentInstrOffset, $"MVN{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        _reg[rd] = ~op2v;
                        if (s == 1)
                            UpdateFlags(_reg[rd] == 0, _c, (_reg[rd] >> 31) == 1, _v);
                    }
                    break;
            }

            if (rd == 0xF)
                SetPC(_reg[rd]);
        }
        private uint GetOp2Value(bool i, uint op2, bool s)
        {
            if (i)
            {
                var imm = op2 & 0xFF;
                var rot = ((op2 >> 8) & 0xF) * 2;

                return ROR(imm, (int)rot);
            }
            else
            {
                var rm = op2 & 0xF;
                var shift = (op2 >> 4) & 0xFF;

                var stype = (shift >> 1) & 0x3;
                var shiftValue = (shift & 0x1) == 1 ? _reg[(shift >> 4) & 0xF] & 0xFF : shift >> 3;

                return Shift(_reg[rm], stype, shiftValue, s);
            }
        }
        #endregion

        #region Single Data Transfer
        private void SingleDataTransfer(uint instruction)
        {
            var i = (instruction >> 25) & 0x1;
            var p = (instruction >> 24) & 0x1;
            var u = (instruction >> 23) & 0x1;
            var b = (instruction >> 22) & 0x1;
            var w = (instruction >> 21) & 0x1;
            var l = (instruction >> 20) & 0x1;
            var rn = (instruction >> 16) & 0xF;
            var rd = (instruction >> 12) & 0xF;
            var offset = instruction & 0xFFF;

            if (i == 1)
            {
                var rm = offset & 0xF;
                var shift = (offset >> 4) & 0xFF;

                var stype = (shift >> 1) & 0x3;
                var shiftValue = (shift & 0x1) == 1 ? _reg[(shift >> 4) & 0xF] & 0xFF : shift >> 3;

                offset = Shift(_reg[offset & 0xF], stype, shiftValue, false);
            }

            if (_currentlyPrinting)
                if (offset != 0)
                    Disassemble(_currentInstrOffset, $"{(l == 1 ? "LDR" : "STR")}{(b == 1 ? "B" : "")}{_currentCondition} R{rd}, [R{rn}{(i == 1 ? $", #{offset}]" : (p == 1 ? $", #{offset}]!" : $"], #{offset}"))}");
                else
                    Disassemble(_currentInstrOffset, $"{(l == 1 ? "LDR" : "STR")}{(b == 1 ? "B" : "")}{_currentCondition} R{rd}, [R{rn}]{(p == 1 ? "!" : "")}");
            else
            {

                var baseAddr = _reg[rn];
                if (p == 1)
                    if (u == 1)
                        baseAddr += offset;
                    else
                        baseAddr -= offset;

                if (l == 1)
                {
                    if (b == 1)
                        _reg[rd] = _mem[baseAddr];
                    else
                        _reg[rd] = ReadUInt32(baseAddr);

                    if (rd == 0xF)
                        SetPC(_reg[rd]);
                }
                else
                {
                    if (b == 1)
                        _mem[baseAddr] = (byte)_reg[rd];
                    else
                    {
                        WriteUInt32(baseAddr, _reg[rd]);
                    }
                }

                if (p == 0)
                    if (u == 1)
                        baseAddr += offset;
                    else
                        baseAddr -= offset;

                if (w == 1)
                    _reg[rn] = baseAddr;
            }
        }
        #endregion

        #region Block Data Transfer
        private void BlockDataTransfer(uint instruction)
        {
            var p = (instruction >> 24) & 0x1;
            var u = (instruction >> 23) & 0x1;
            var s = (instruction >> 22) & 0x1;
            var w = (instruction >> 21) & 0x1;
            var l = (instruction >> 20) & 0x1;
            var rn = (instruction >> 16) & 0xF;
            var list = instruction & 0xFFFF;

            //TODO: Finish Block Data Transfer

            long address = _reg[rn];

            for (int i = 0; i < 16; i++)
            {
                if (((list >> i) & 0x1) == 1)
                {
                    if (p == 1 || u == 0)
                        address += 4;

                    if (l == 1)
                    {
                        _reg[i] = ReadUInt32((uint)address);
                        if (i == 15)
                            SetPC(_reg[i]);
                    }
                    else
                        WriteUInt32((uint)address, _reg[i]);

                    if (p == 0 || u == 0)
                        address += 4;
                }
            }
        }
        #endregion
    }
}
