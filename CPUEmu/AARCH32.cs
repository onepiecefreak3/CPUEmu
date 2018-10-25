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
    public class AARCH32 : Emulator
    {
        private bool _z;
        private bool _c;
        private bool _n;
        private bool _v;

        private uint[] _reg = new uint[16];

        private uint _pc { get => _reg[15]; set => _reg[15] = value; }
        private uint _lr { get => _reg[14]; set => _reg[14] = value; }
        private uint _sp { get => _reg[13]; set => _reg[13] = value; }

        private string _currentCondition;
        private long _currentInstrOffset;
        private bool _currentlyPrinting;

        private uint _binaryEntry;
        private uint _binarySize;

        Queue<uint> _instrBuffer = new Queue<uint>();

        #region Abstract
        public AARCH32(byte[] input, int binaryEntry, int stackAddress, int stackSize) : base(input, binaryEntry, stackAddress, stackSize)
        {
            _pc = (uint)binaryEntry;
            _sp = (uint)stackAddress;

            _binaryEntry = (uint)binaryEntry;
            _binarySize = (uint)input.Length;

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
        public override event DisassembleEventHandler Disassemble;

        public override void ExecuteNextInstruction()
        {
            _currentlyPrinting = false;
            var instruction = _instrBuffer.Dequeue();

            if (CheckConditions((byte)(instruction >> 28)))
                HandleInstructionType(instruction);

            ReadNextInstruction();
        }

        public override void DisassembleInstructions(long offset, int count)
        {
            _currentlyPrinting = true;

            for (uint i = (uint)offset; i < offset + count * 4; i += 4)
            {
                if (i >= _binaryEntry && i < _binaryEntry + _binarySize)
                {
                    _currentInstrOffset = i;

                    var instruction = ReadUInt32(i);
                    CheckConditions((byte)(instruction >> 28));
                    HandleInstructionType(instruction);
                }
            }
        }

        public override List<(string flagName, long value)> RetrieveFlags()
        {
            return new List<(string flagName, long value)>
            {
                ("Z", _z ? 1 : 0),
                ("C", _c ? 1 : 0),
                ("N", _n ? 1 : 0),
                ("V", _v ? 1 : 0)
            };
        }

        public override List<(string registerName, long value)> RetrieveRegisters()
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

        private bool CheckConditions(byte condition)
        {
            switch (condition)
            {
                case 0:
                    _currentCondition = "EQ";
                    return _z;
                case 1:
                    _currentCondition = "NE";
                    return !_z;
                case 2:
                    _currentCondition = "CS";
                    return _c;
                case 3:
                    _currentCondition = "CC";
                    return !_c;
                case 4:
                    _currentCondition = "MI";
                    return _n;
                case 5:
                    _currentCondition = "PL";
                    return !_n;
                case 6:
                    _currentCondition = "VS";
                    return _v;
                case 7:
                    _currentCondition = "VC";
                    return !_v;
                case 8:
                    _currentCondition = "HI";
                    return _c && !_z;
                case 9:
                    _currentCondition = "LS";
                    return !_c || _z;
                case 10:
                    _currentCondition = "GE";
                    return _n == _v;
                case 11:
                    _currentCondition = "LT";
                    return _n != _v;
                case 12:
                    _currentCondition = "GT";
                    return !_z && (_n == _v);
                case 13:
                    _currentCondition = "LE";
                    return _z || (_n != _v);
                case 14:
                    _currentCondition = "";
                    return true;

                default:
                    _currentCondition = "";
                    Log?.Invoke(this, $"Unknown condition 0x{condition:X1}. Ignore instruction.");
                    return false;
            }
        }

        private void HandleInstructionType(uint instruction)
        {
            switch (GetInstructionType(instruction))
            {
                case InstructionType.DataProcessing:
                    DataProcessing(instruction);
                    if (!_currentlyPrinting) Log?.Invoke(this, "Data Processing");
                    break;

                case InstructionType.Multiply:
                    break;

                case InstructionType.MultiplyLong:
                    break;

                case InstructionType.SingleDataSwap:
                    break;

                case InstructionType.BranchExchange:
                    BranchExchange(instruction);
                    if (!_currentlyPrinting) Log?.Invoke(this, "Branch and Exchange");
                    break;

                case InstructionType.HalfwordDataTransferReg:
                    break;

                case InstructionType.HalfwordDataTransferImm:
                    break;

                case InstructionType.SingleDataTransfer:
                    SingleDataTransfer(instruction);
                    if (!_currentlyPrinting) Log?.Invoke(this, "Single Data Transfer");
                    break;

                case InstructionType.BlockDataTransfer:
                    BlockDataTransfer(instruction);
                    if (!_currentlyPrinting) Log?.Invoke(this, "Block Data Transfer");
                    break;

                case InstructionType.Branch:
                    Branch(instruction);
                    if (!_currentlyPrinting) Log?.Invoke(this, "Branch");
                    break;

                case InstructionType.CoprocessorDataTransfer:
                    break;

                case InstructionType.CoprocessorDataOperation:
                    break;

                case InstructionType.CoprocessorRegTransfer:
                    break;

                case InstructionType.SoftwareInterrupt:
                    if (!_currentlyPrinting) Log?.Invoke(this, $"Software interrupt at PC 0x{_pc - 4:X8}");
                    break;

                case InstructionType.Undefined:
                default:
                    if (!_currentlyPrinting) Log?.Invoke(this, $"Instruction 0x{instruction:X8} undefined at PC 0x{_pc - 4:X8}");
                    break;
            }
        }

        private InstructionType GetInstructionType(uint instruction)
        {
            var check = instruction & 0x0C000000;
            if (check == 0x0)
            {
                if ((instruction & 0x03FFFFF0) == 0x012FFF10)
                    return InstructionType.BranchExchange;
                if ((instruction & 0x03B00FF0) == 0x01000090)
                    return InstructionType.SingleDataSwap;
                if ((instruction & 0x02400F90) == 0x00000090)
                    return InstructionType.HalfwordDataTransferReg;
                if ((instruction & 0x03C00090) == 0x00000090)
                    return InstructionType.Multiply;
                if ((instruction & 0x03800090) == 0x00800090)
                    return InstructionType.MultiplyLong;
                if ((instruction & 0x02400090) == 0x00400090)
                    return InstructionType.HalfwordDataTransferImm;

                return InstructionType.DataProcessing;
            }
            if (check == 0x04000000)
            {
                if ((instruction & 0x02000010) == 0x02000010)
                    return InstructionType.Undefined;

                return InstructionType.SingleDataTransfer;
            }
            if (check == 0x08000000)
            {
                if ((instruction & 0x02000000) == 0x0)
                    return InstructionType.BlockDataTransfer;

                return InstructionType.Branch;
            }
            if (check == 0x0C000000)
            {
                if ((instruction & 0x02000000) == 0x0)
                    return InstructionType.CoprocessorDataTransfer;
                if ((instruction & 0x03000010) == 0x02000000)
                    return InstructionType.CoprocessorDataOperation;
                if ((instruction & 0x03000010) == 0x02000010)
                    return InstructionType.CoprocessorRegTransfer;

                return InstructionType.SoftwareInterrupt;
            }

            return InstructionType.Undefined;
        }

        private enum InstructionType
        {
            DataProcessing,
            Multiply,
            MultiplyLong,
            SingleDataSwap,
            BranchExchange,
            HalfwordDataTransferReg,
            HalfwordDataTransferImm,
            SingleDataTransfer,
            Undefined,
            BlockDataTransfer,
            Branch,
            CoprocessorDataTransfer,
            CoprocessorDataOperation,
            CoprocessorRegTransfer,
            SoftwareInterrupt
        }

        private uint ReadUInt32(uint offset) => (uint)(_mem[offset] | (_mem[offset + 1] << 8) | (_mem[offset + 2] << 16) | (_mem[offset + 3] << 24));
        private void WriteUInt32(uint offset, uint value)
        {
            _mem[offset] = (byte)value;
            _mem[offset + 1] = (byte)(value >> 8);
            _mem[offset + 2] = (byte)(value >> 16);
            _mem[offset + 3] = (byte)(value >> 24);
        }

        private void ReadNextInstruction()
        {
            if (_pc >= _binaryEntry && _pc < _binaryEntry + _binarySize)
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
                        Disassemble?.Invoke(this, _currentInstrOffset, $"AND{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
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
                        Disassemble?.Invoke(this, _currentInstrOffset, $"EOR{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
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
                        Disassemble?.Invoke(this, _currentInstrOffset, $"SUB{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
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
                        Disassemble?.Invoke(this, _currentInstrOffset, $"RSB{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}, R{rn}");
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
                        Disassemble?.Invoke(this, _currentInstrOffset, $"ADD{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
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
                        Disassemble?.Invoke(this, _currentInstrOffset, $"ADC{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
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
                        Disassemble?.Invoke(this, _currentInstrOffset, $"SBC{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
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
                        Disassemble?.Invoke(this, _currentInstrOffset, $"RSC{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}, R{rn}");
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
                        Disassemble?.Invoke(this, _currentInstrOffset, $"TST{_currentCondition} R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        var res = _reg[rn] & op2v;
                        UpdateFlags(res == 0, _c, (res >> 31) == 1, _v);
                    }
                    break;

                //TEQ
                case 9:
                    if (_currentlyPrinting)
                        Disassemble?.Invoke(this, _currentInstrOffset, $"TEQ{_currentCondition} R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        var res = _reg[rn] ^ op2v;
                        UpdateFlags(res == 0, _c, (res >> 31) == 1, _v);
                    }
                    break;

                //CMP
                case 10:
                    if (_currentlyPrinting)
                        Disassemble?.Invoke(this, _currentInstrOffset, $"CMP{_currentCondition} R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        var res = _reg[rn] - op2v;
                        UpdateFlags(res == 0, res < _reg[rn], (res >> 31) == 1, (res >> 31) != (_reg[rn] >> 31));
                    }
                    break;

                //CMN
                case 11:
                    if (_currentlyPrinting)
                        Disassemble?.Invoke(this, _currentInstrOffset, $"CMN{_currentCondition} R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
                    else
                    {
                        var res = _reg[rn] + op2v;
                        UpdateFlags(res == 0, res < _reg[rn], (res >> 31) == 1, (res >> 31) != (_reg[rn] >> 31));
                    }
                    break;

                //ORR
                case 12:
                    if (_currentlyPrinting)
                        Disassemble?.Invoke(this, _currentInstrOffset, $"ORR{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
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
                        Disassemble?.Invoke(this, _currentInstrOffset, $"MOV{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
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
                        Disassemble?.Invoke(this, _currentInstrOffset, $"BIC{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, R{rn}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
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
                        Disassemble?.Invoke(this, _currentInstrOffset, $"MVN{(s == 1 ? "S" : "")}{_currentCondition} R{rd}, {(i == 1 ? $"#{op2v}" : $"R{op2 & 0xF}")}");
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
                    Disassemble?.Invoke(this, _currentInstrOffset, $"{(l == 1 ? "LDR" : "STR")}{(b == 1 ? "B" : "")}{_currentCondition} R{rd}, [R{rn}{(i == 1 ? $", #{offset}]" : (p == 1 ? $", #{offset}]!" : $"], #{offset}"))}");
                else
                    Disassemble?.Invoke(this, _currentInstrOffset, $"{(l == 1 ? "LDR" : "STR")}{(b == 1 ? "B" : "")}{_currentCondition} R{rd}, [R{rn}]{(p == 1 ? "!" : "")}");
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

        #region Branch
        private void Branch(uint instruction)
        {
            var offset = (instruction & 0xFFFFFF) << 2;
            var l = (instruction >> 24) & 0x1;

            //sign extend offset
            var sign = (offset >> 25) & 0x1;
            for (int i = 26; i < 32; i++) offset |= sign << i;

            if (_currentlyPrinting)
                Disassemble?.Invoke(this, _currentInstrOffset, $"B{(l == 1 ? "L" : "")}{_currentCondition} 0x{_currentInstrOffset + 8 + offset:X2}");
            else
            {
                if (l == 1)
                    _lr = _pc - 4;

                SetPC((uint)((int)_pc + (int)offset));
            }
        }

        private void BranchExchange(uint instruction)
        {

        }
        #endregion

        #region Shifter
        private uint ROR(uint value, int rot) => (uint)((value >> rot) | ((value & ((1 << rot) - 1)) << (32 - rot)));

        private uint Shift(uint value, uint type, uint count, bool s)
        {
            switch (type)
            {
                //LSL
                case 0:
                    if (count != 0 && s)
                        _c = (((int)value >> (int)(32 - count)) & 0x1) == 1;
                    return value << (int)count;

                //LSR
                case 1:
                    if (count != 0 && s)
                        _c = (((int)value >> (int)(count - 1)) & 0x1) == 1;
                    return value >> (int)count;

                //ASR
                case 2:
                    if (count != 0 && s)
                        _c = (((int)value >> (int)(count - 1)) & 0x1) == 1;
                    return (uint)((int)value >> (int)count);

                //ROR
                case 3:
                    if (count != 0 && s)
                        _c = (((int)value >> (int)(count - 1)) & 0x1) == 1;
                    return ROR(value, (int)count);

                default:
                    return 0;
            }
        }
        #endregion
    }
}
