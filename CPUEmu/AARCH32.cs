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
        private BinaryReader _binary;
        private byte[] _mem = ArrayPool<byte>.Shared.Rent(0x100000);  //1MB Memory

        private bool _z;
        private bool _c;
        private bool _n;
        private bool _v;
        private uint _pc { get => _reg[15]; set => _reg[15] = value; }
        private uint _lr { get => _reg[14]; set => _reg[14] = value; }
        private uint[] _reg = new uint[16];

        Queue<uint> _instrBuffer = new Queue<uint>();

        #region UI Exposition
        public override Dictionary<string, long> RetrieveFlags()
        {
            return new Dictionary<string, long>
            {
                ["Z"] = _z ? 1 : 0,
                ["C"] = _c ? 1 : 0,
                ["N"] = _n ? 1 : 0,
                ["V"] = _v ? 1 : 0,
            };
        }

        public override Dictionary<string, long> RetrieveRegisters()
        {
            return new Dictionary<string, long>
            {
                ["R0"] = _reg[0],
                ["R1"] = _reg[1],
                ["R2"] = _reg[2],
                ["R3"] = _reg[3],
                ["R4"] = _reg[4],
                ["R5"] = _reg[5],
                ["R6"] = _reg[6],
                ["R7"] = _reg[7],
                ["R8"] = _reg[8],
                ["R9"] = _reg[9],
                ["R10"] = _reg[10],
                ["R11"] = _reg[11],
                ["R12"] = _reg[12],
                ["R13"] = _reg[13],
                ["LR"] = _reg[14],
                ["PC"] = _reg[15],
            };
        }
        #endregion

        #region Properties
        public override string Name => "AARCH32";

        public override int BitVersion => 32;

        public override bool IsFinished => !_instrBuffer.Any();
        #endregion

        public override event LogEventHandler Log;

        public override void ExecuteNextInstruction()
        {
            var instruction = _instrBuffer.Dequeue();

            if (CheckConditions((byte)(instruction >> 28)))
                switch (GetInstructionType(instruction))
                {
                    case InstructionType.DataProcessing:
                        DataProcessing(instruction);
                        Log(this, "Data Processing");
                        break;

                    case InstructionType.Multiply:
                        break;

                    case InstructionType.MultiplyLong:
                        break;

                    case InstructionType.SingleDataSwap:
                        break;

                    case InstructionType.BranchExchange:
                        BranchExchange(instruction);
                        Log(this, "Branch and Exchange");
                        break;

                    case InstructionType.HalfwordDataTransferReg:
                        break;

                    case InstructionType.HalfwordDataTransferImm:
                        break;

                    case InstructionType.SingleDataTransfer:
                        SingleDataTransfer(instruction);
                        Log(this, "Single Data Transfer");
                        break;

                    case InstructionType.BlockDataTransfer:
                        break;

                    case InstructionType.Branch:
                        Branch(instruction);
                        Log(this, "Branch");
                        break;

                    case InstructionType.CoprocessorDataTransfer:
                        break;

                    case InstructionType.CoprocessorDataOperation:
                        break;

                    case InstructionType.CoprocessorRegTransfer:
                        break;

                    case InstructionType.SoftwareInterrupt:
                        Log(this, $"Software interrupt at PC 0x{_pc - 4:X8}");
                        break;

                    case InstructionType.Undefined:
                    default:
                        Log(this, $"Instruction 0x{instruction:X8} undefined at PC 0x{_pc - 4:X8}");
                        break;
                }

            if (_binary.BaseStream.Position < _binary.BaseStream.Length)
                _instrBuffer.Enqueue(_binary.ReadUInt32());
            _pc += 4;
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

        private bool CheckConditions(byte condition)
        {
            switch (condition)
            {
                case 0:
                    return _z;
                case 1:
                    return !_z;
                case 2:
                    return _c;
                case 3:
                    return !_c;
                case 4:
                    return _n;
                case 5:
                    return !_n;
                case 6:
                    return _v;
                case 7:
                    return !_v;
                case 8:
                    return _c && !_z;
                case 9:
                    return !_c || _z;
                case 10:
                    return _n == _v;
                case 11:
                    return _n != _v;
                case 12:
                    return !_z && (_n == _v);
                case 13:
                    return _z || (_n != _v);
                case 14:
                    return true;

                default:
                    Log(this, $"Unknown condition 0x{condition:X1}. Ignore instruction.");
                    return false;
            }
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

            switch (opcode)
            {
                //AND
                case 0:
                    _reg[rd] = _reg[rn] & GetOp2Value(i == 1, op2, s == 1);
                    if (s == 1)
                    {
                        _z = _reg[rd] == 0;
                        _n = (_reg[rd] >> 31) == 1;
                    }
                    break;

                //EOR
                case 1:
                    _reg[rd] = _reg[rn] ^ GetOp2Value(i == 1, op2, s == 1);
                    if (s == 1)
                    {
                        _z = _reg[rd] == 0;
                        _n = (_reg[rd] >> 31) == 1;
                    }
                    break;

                //SUB
                case 2:
                    _reg[rd] = _reg[rn] - GetOp2Value(i == 1, op2, s == 1);
                    if (s == 1)
                    {
                        _v = (_reg[rd] >> 31) != (_reg[rn] >> 31);
                        _z = _reg[rd] == 0;
                        _n = (_reg[rd] >> 31) == 1;
                        _c = _reg[rd] < _reg[rn];
                    }
                    break;

                //RSB
                case 3:
                    var op2v = GetOp2Value(i == 1, op2, s == 1);
                    _reg[rd] = op2v - _reg[rn];
                    if (s == 1)
                    {
                        _v = (_reg[rd] >> 31) != (op2v >> 31);
                        _z = _reg[rd] == 0;
                        _n = (_reg[rd] >> 31) == 1;
                        _c = _reg[rd] < op2v;
                    }
                    break;

                //ADD
                case 4:
                    _reg[rd] = _reg[rn] + GetOp2Value(i == 1, op2, s == 1);
                    if (s == 1)
                    {
                        _v = (_reg[rd] >> 31) != (_reg[rn] >> 31);
                        _z = _reg[rd] == 0;
                        _n = (_reg[rd] >> 31) == 1;
                        _c = _reg[rd] < _reg[rn];
                    }
                    break;

                //ADC
                case 5:
                    _reg[rd] = _reg[rn] + GetOp2Value(i == 1, op2, s == 1);
                    if (s == 1)
                    {
                        _v = (_reg[rd] >> 31) != (_reg[rn] >> 31);
                        _z = _reg[rd] == 0;
                        _n = (_reg[rd] >> 31) == 1;
                        _c = _reg[rd] < _reg[rn];
                    }
                    _reg[rd] += (uint)(_c ? 1 : 0);
                    break;

                //SBC
                case 6:
                    _reg[rd] = _reg[rn] + GetOp2Value(i == 1, op2, s == 1);
                    if (s == 1)
                    {
                        _v = (_reg[rd] >> 31) != (_reg[rn] >> 31);
                        _z = _reg[rd] == 0;
                        _n = (_reg[rd] >> 31) == 1;
                        _c = _reg[rd] < _reg[rn];
                    }
                    _reg[rd] += (uint)(_c ? 0 : -1);
                    break;

                //RSC
                case 7:
                    op2v = GetOp2Value(i == 1, op2, s == 1);
                    _reg[rd] = op2v - _reg[rn];
                    if (s == 1)
                    {
                        _v = (_reg[rd] >> 31) != (op2v >> 31);
                        _z = _reg[rd] == 0;
                        _n = (_reg[rd] >> 31) == 1;
                        _c = _reg[rd] < op2v;
                    }
                    _reg[rd] += (uint)(_c ? 0 : -1);
                    break;

                //TST
                case 8:
                    var res = _reg[rn] & GetOp2Value(i == 1, op2, s == 1);
                    if (s == 1)
                    {
                        _z = res == 0;
                        _n = (res >> 31) == 1;
                    }
                    break;

                //TEQ
                case 9:
                    res = _reg[rn] ^ GetOp2Value(i == 1, op2, s == 1);
                    if (s == 1)
                    {
                        _z = res == 0;
                        _n = (res >> 31) == 1;
                    }
                    break;

                //CMP
                case 10:
                    res = _reg[rn] - GetOp2Value(i == 1, op2, s == 1);
                    if (s == 1)
                    {
                        _v = (res >> 31) != (_reg[rn] >> 31);
                        _z = res == 0;
                        _n = (res >> 31) == 1;
                        _c = res < _reg[rn];
                    }
                    break;

                //CMN
                case 11:
                    res = _reg[rn] + GetOp2Value(i == 1, op2, s == 1);
                    if (s == 1)
                    {
                        _v = (res >> 31) != (_reg[rn] >> 31);
                        _z = res == 0;
                        _n = (res >> 31) == 1;
                        _c = res < _reg[rn];
                    }
                    break;

                //ORR
                case 12:
                    _reg[rd] = _reg[rn] | GetOp2Value(i == 1, op2, s == 1);
                    if (s == 1)
                    {
                        _z = _reg[rd] == 0;
                        _n = (_reg[rd] >> 31) == 1;
                    }
                    break;

                //MOV
                case 13:
                    _reg[rd] = GetOp2Value(i == 1, op2, s == 1);
                    if (s == 1)
                    {
                        _z = _reg[rd] == 0;
                        _n = (_reg[rd] >> 31) == 1;
                    }
                    break;

                //BIC
                case 14:
                    _reg[rd] = _reg[rn] & ~GetOp2Value(i == 1, op2, s == 1);
                    if (s == 1)
                    {
                        _z = _reg[rd] == 0;
                        _n = (_reg[rd] >> 31) == 1;
                    }
                    break;

                //MVN
                case 15:
                    op2v = GetOp2Value(i == 1, op2, s == 1);
                    _reg[rd] = ~op2v;
                    if (s == 1)
                    {
                        _z = _reg[rd] == 0;
                        _n = (_reg[rd] >> 31) == 1;
                    }
                    break;
            }
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
                    _reg[rd] = (uint)(_mem[baseAddr] | (_mem[baseAddr + 1] << 8) | (_mem[baseAddr + 2] << 16) | (_mem[baseAddr + 3] << 24));
            }
            else
            {
                if (b == 1)
                    _mem[baseAddr] = (byte)_reg[rd];
                else
                {
                    _mem[baseAddr] = (byte)_reg[rd];
                    _mem[baseAddr + 1] = (byte)(_reg[rd] >> 8);
                    _mem[baseAddr + 2] = (byte)(_reg[rd] >> 16);
                    _mem[baseAddr + 3] = (byte)(_reg[rd] >> 24);
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
        #endregion

        #region Branch
        private void Branch(uint instruction)
        {
            var offset = (instruction & 0xFFFFFF) << 2;
            var l = (instruction >> 24) & 0x1;

            //sign extend offset
            var sign = (offset >> 25) & 0x1;
            for (int i = 26; i < 32; i++) offset |= sign << i;

            if (l == 1)
                _lr = _pc - 4;

            SetPC((uint)((int)_pc + (int)offset));
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

        private void SetPC(uint offset)
        {
            _binary.BaseStream.Position = _pc = offset;

            _instrBuffer.Clear();
            if (_binary.BaseStream.Position < _binary.BaseStream.Length)
                _instrBuffer.Enqueue(_binary.ReadUInt32());
            if (_binary.BaseStream.Position < _binary.BaseStream.Length)
                _instrBuffer.Enqueue(_binary.ReadUInt32());
            _pc += 8;
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

        public override void Open(Stream input)
        {
            _binary = new BinaryReader(input);

            if (_binary.BaseStream.Position < _binary.BaseStream.Length)
                _instrBuffer.Enqueue(_binary.ReadUInt32());
            if (_binary.BaseStream.Position < _binary.BaseStream.Length)
                _instrBuffer.Enqueue(_binary.ReadUInt32());
            _pc = 8;
        }
    }
}
