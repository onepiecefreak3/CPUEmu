using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu
{
    public partial class AARCH32
    {
        private void DataProcessing(uint instruction)
        {
            var desc = DescribeDataProcessing(instruction);

            switch (desc.opcode)
            {
                case 0:
                    HandleAND(desc);
                    break;
                case 1:
                    HandleEOR(desc);
                    break;
                case 2:
                    HandleSUB(desc);
                    break;
                case 3:
                    HandleRSB(desc);
                    break;
                case 4:
                    HandleADD(desc);
                    break;
                case 5:
                    HandleADC(desc);
                    break;
                case 6:
                    HandleSBC(desc);
                    break;
                case 7:
                    HandleRSC(desc);
                    break;
                case 8:
                    HandleTST(desc);
                    break;
                case 9:
                    HandleTEQ(desc);
                    break;
                case 10:
                    HandleCMP(desc);
                    break;
                case 11:
                    HandleCMN(desc);
                    break;
                case 12:
                    HandleORR(desc);
                    break;
                case 13:
                    HandleMOV(desc);
                    break;
                case 14:
                    HandleBIC(desc);
                    break;
                case 15:
                    HandleMVN(desc);
                    break;
            }

            if (desc.rd == 0xF)
                SetPC(_reg[desc.rd]);
        }

        private string DisassembleDataProcessing(uint instruction)
        {
            var desc = DescribeDataProcessing(instruction);

            switch (desc.opcode)
            {
                case 0:
                    return DisassembleAND(desc);
                case 1:
                    return DisassembleEOR(desc);
                case 2:
                    return DisassembleSUB(desc);
                case 3:
                    return DisassembleRSB(desc);
                case 4:
                    return DisassembleADD(desc);
                case 5:
                    return DisassembleADC(desc);
                case 6:
                    return DisassembleSBC(desc);
                case 7:
                    return DisassembleRSC(desc);
                case 8:
                    return DisassembleTST(desc);
                case 9:
                    return DisassembleTEQ(desc);
                case 10:
                    return DisassembleCMP(desc);
                case 11:
                    return DisassembleCMN(desc);
                case 12:
                    return DisassembleORR(desc);
                case 13:
                    return DisassembleMOV(desc);
                case 14:
                    return DisassembleBIC(desc);
                case 15:
                    return DisassembleMVN(desc);
            }

            return String.Empty;
        }

        #region Handle OpCodes
        private void HandleAND(DataProcessorDescriptor desc)
        {
            _reg[desc.rd] = (uint)(_reg[desc.rn] & desc.operand2Value);
            if (desc.s) SetFlagsLogical(_reg[desc.rd]);
        }

        private void HandleEOR(DataProcessorDescriptor desc)
        {
            _reg[desc.rd] = (uint)(_reg[desc.rn] ^ desc.operand2Value);
            if (desc.s) SetFlagsLogical(_reg[desc.rd]);
        }

        private void HandleSUB(DataProcessorDescriptor desc)
        {
            _reg[desc.rd] = (uint)(_reg[desc.rn] - desc.operand2Value);
            if (desc.s) SetFlagsArithmethic(_reg[desc.rd], _reg[desc.rn], (uint)desc.operand2Value, true);
        }

        private void HandleRSB(DataProcessorDescriptor desc)
        {
            _reg[desc.rd] = (uint)(desc.operand2Value - _reg[desc.rn]);
            if (desc.s) SetFlagsArithmethic(_reg[desc.rd], (uint)desc.operand2Value, _reg[desc.rn], true);
        }

        private void HandleADD(DataProcessorDescriptor desc)
        {
            _reg[desc.rd] = (uint)(_reg[desc.rn] + desc.operand2Value);
            if (desc.s) SetFlagsArithmethic(_reg[desc.rd], _reg[desc.rn], (uint)desc.operand2Value, false);
        }

        private void HandleADC(DataProcessorDescriptor desc)
        {
            _reg[desc.rd] = (uint)(_reg[desc.rn] + desc.operand2Value);
            _reg[desc.rd] += (uint)(_c ? 1 : 0);
            if (desc.s) SetFlagsArithmethic(_reg[desc.rd], _reg[desc.rn], (uint)desc.operand2Value, _c);
        }

        private void HandleSBC(DataProcessorDescriptor desc)
        {
            _reg[desc.rd] = (uint)(_reg[desc.rn] - desc.operand2Value);
            _reg[desc.rd] += (uint)(_c ? 0 : -1);
            if (desc.s) SetFlagsArithmethic(_reg[desc.rd], _reg[desc.rn], (uint)desc.operand2Value, _c);
        }

        private void HandleRSC(DataProcessorDescriptor desc)
        {
            _reg[desc.rd] = (uint)(desc.operand2Value - _reg[desc.rn]);
            _reg[desc.rd] += (uint)(_c ? 0 : -1);
            if (desc.s) SetFlagsArithmethic(_reg[desc.rd], (uint)desc.operand2Value, _reg[desc.rn], _c);
        }

        private void HandleTST(DataProcessorDescriptor desc)
        {
            var res = _reg[desc.rn] & desc.operand2Value;
            SetFlagsLogical((uint)res);
        }

        private void HandleTEQ(DataProcessorDescriptor desc)
        {
            var res = _reg[desc.rn] ^ desc.operand2Value;
            SetFlagsLogical((uint)res);
        }

        private void HandleCMP(DataProcessorDescriptor desc)
        {
            var res = _reg[desc.rn] - desc.operand2Value;
            SetFlagsArithmethic((uint)res, _reg[desc.rn], (uint)desc.operand2Value, true);
        }

        private void HandleCMN(DataProcessorDescriptor desc)
        {
            var res = _reg[desc.rn] + desc.operand2Value;
            SetFlagsArithmethic((uint)res, _reg[desc.rn], (uint)desc.operand2Value, false);
        }

        private void HandleORR(DataProcessorDescriptor desc)
        {
            _reg[desc.rd] = (uint)(_reg[desc.rn] | desc.operand2Value);
            if (desc.s) SetFlagsLogical(_reg[desc.rd]);
        }

        private void HandleMOV(DataProcessorDescriptor desc)
        {
            _reg[desc.rd] = (uint)desc.operand2Value;
            if (desc.s) SetFlagsLogical(_reg[desc.rd]);
        }

        private void HandleBIC(DataProcessorDescriptor desc)
        {
            _reg[desc.rd] = (uint)(_reg[desc.rn] & ~desc.operand2Value);
            if (desc.s) SetFlagsLogical(_reg[desc.rd]);
        }

        private void HandleMVN(DataProcessorDescriptor desc)
        {
            _reg[desc.rd] = (uint)~desc.operand2Value;
            if (desc.s) SetFlagsLogical(_reg[desc.rd]);
        }
        #endregion

        #region Set Flags
        private void SetFlagsLogical(uint result)
        {
            UpdateFlags(result == 0, _c, (result >> 31) == 1, _v);
        }

        private void SetFlagsArithmethic(uint result, uint value, uint value2, bool carryIn)
        {
            UpdateFlags(
                result == 0,
                (!carryIn) ? result < value : result <= value,
                (result >> 31) == 1,
                ((value >> 31) != (value2 >> 31)) && ((value >> 31) != (result >> 31))
                );
        }
        #endregion

        #region Disassemble OpCodes
        private string DisassembleAND(DataProcessorDescriptor desc)
        {
            return $"AND{(desc.s ? "S" : "")}{_currentCondition} R{desc.rd}, R{desc.rn}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}";
        }

        private string DisassembleEOR(DataProcessorDescriptor desc)
        {
            return $"EOR{(desc.s ? "S" : "")}{_currentCondition} R{desc.rd}, R{desc.rn}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}";
        }

        private string DisassembleSUB(DataProcessorDescriptor desc)
        {
            return $"SUB{(desc.s ? "S" : "")}{_currentCondition} R{desc.rd}, R{desc.rn}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}";
        }

        private string DisassembleRSB(DataProcessorDescriptor desc)
        {
            return $"RSB{(desc.s ? "S" : "")}{_currentCondition} R{desc.rd}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}, R{desc.rn}";
        }

        private string DisassembleADD(DataProcessorDescriptor desc)
        {
            return $"ADD{(desc.s ? "S" : "")}{_currentCondition} R{desc.rd}, R{desc.rn}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}";
        }

        private string DisassembleADC(DataProcessorDescriptor desc)
        {
            return $"ADC{(desc.s ? "S" : "")}{_currentCondition} R{desc.rd}, R{desc.rn}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}";
        }

        private string DisassembleSBC(DataProcessorDescriptor desc)
        {
            return $"SBC{(desc.s ? "S" : "")}{_currentCondition} R{desc.rd}, R{desc.rn}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}";
        }

        private string DisassembleRSC(DataProcessorDescriptor desc)
        {
            return $"RSC{(desc.s ? "S" : "")}{_currentCondition} R{desc.rd}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}, R{desc.rn}";
        }

        private string DisassembleTST(DataProcessorDescriptor desc)
        {
            return $"TST{_currentCondition} R{desc.rn}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}";
        }

        private string DisassembleTEQ(DataProcessorDescriptor desc)
        {
            return $"TEQ{_currentCondition} R{desc.rn}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}";
        }

        private string DisassembleCMP(DataProcessorDescriptor desc)
        {
            return $"CMP{_currentCondition} R{desc.rn}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}";
        }

        private string DisassembleCMN(DataProcessorDescriptor desc)
        {
            return $"CMN{_currentCondition} R{desc.rn}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}";
        }

        private string DisassembleORR(DataProcessorDescriptor desc)
        {
            return $"ORR{(desc.s ? "S" : "")}{_currentCondition} R{desc.rd}, R{desc.rn}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}";
        }

        private string DisassembleMOV(DataProcessorDescriptor desc)
        {
            return $"MOV{(desc.s ? "S" : "")}{_currentCondition} R{desc.rd}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}";
        }

        private string DisassembleBIC(DataProcessorDescriptor desc)
        {
            return $"BIC{(desc.s ? "S" : "")}{_currentCondition} R{desc.rd}, R{desc.rn}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}";
        }

        private string DisassembleMVN(DataProcessorDescriptor desc)
        {
            return $"MVN{(desc.s ? "S" : "")}{_currentCondition} R{desc.rd}, {(desc.i ? $"#{desc.operand2Value}" : $"R{desc.operand2 & 0xF}")}";
        }
        #endregion

        #region Describe
        private DataProcessorDescriptor DescribeDataProcessing(uint instruction)
        {
            var operand2 = instruction & 0xFFF;

            var res = new DataProcessorDescriptor
            {
                i = ((instruction >> 25) & 0x1) == 1,
                opcode = (instruction >> 21) & 0xF,
                s = ((instruction >> 20) & 0x1) == 1,
                rn = (instruction >> 16) & 0xF,
                rd = (instruction >> 12) & 0xF,
                operand2 = operand2
            };

            var logical = res.opcode == 0 || res.opcode == 1 || res.opcode == 8 || res.opcode == 9 || res.opcode == 0xc || res.opcode == 0xd || res.opcode == 0xe || res.opcode == 0xf;

            res.operand2Value = GetOp2Value(res, logical);

            return res;
        }
        private class DataProcessorDescriptor
        {
            public bool i;
            public long opcode;
            public bool s;
            public long rn;
            public long rd;
            public long operand2;
            public long operand2Value;
        }

        private uint GetOp2Value(DataProcessorDescriptor desc, bool l)
        {
            uint shifted;
            bool carry;

            if (desc.i)
            {
                var imm = desc.operand2 & 0xFF;
                var rot = ((desc.operand2 >> 8) & 0xF) * 2;

                shifted = _shifter.ROR((uint)imm, (int)rot, out carry);
                if (rot == 0) carry = _c;
            }
            else
            {
                var rm = desc.operand2 & 0xF;
                var shift = (desc.operand2 >> 4) & 0xFF;

                var stype = (shift >> 1) & 0x3;
                var shiftValue = (shift & 0x1) == 1 ? _reg[(shift >> 4) & 0xF] & 0xFF : shift >> 3;

                shifted = _shifter.ShiftByType((BarrelShifter.ShiftType)stype, _reg[rm], (int)shiftValue, out carry);
                if (shiftValue == 0) carry = _c;
            }

            if (desc.s && l) _c = carry;
            return shifted;
        }
        #endregion
    }
}
