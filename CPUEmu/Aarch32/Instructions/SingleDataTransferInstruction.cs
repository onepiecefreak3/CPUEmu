﻿using System;
using CPUEmu.Interfaces;

namespace CPUEmu.Aarch32.Instructions
{
    class SingleDataTransferInstruction : IInstruction
    {
        private readonly byte _condition;
        private readonly bool _i;
        private readonly bool _p;
        private readonly bool _u;
        private readonly bool _b;
        private readonly bool _w;
        private readonly bool _l;
        private readonly byte _rn;
        private readonly byte _rd;
        private readonly uint _offset;

        public int Position { get; }

        private SingleDataTransferInstruction(int position, byte condition, bool i, bool p, bool u, bool b, bool w, bool l, byte rn, byte rd, uint offset)
        {
            Position = position;
            _condition = condition;
            _i = i;
            _p = p;
            _u = u;
            _b = b;
            _w = w;
            _l = l;
            _rn = rn;
            _rd = rd;
            _offset = offset;
        }

        public static IInstruction Parse(int position, byte condition, uint instruction)
        {
            var i = ((instruction >> 25) & 0x1) == 1;
            var p = ((instruction >> 24) & 0x1) == 1;
            var u = ((instruction >> 23) & 0x1) == 1;
            var b = ((instruction >> 22) & 0x1) == 1;
            var w = ((instruction >> 21) & 0x1) == 1;
            var l = ((instruction >> 20) & 0x1) == 1;
            var rn = (byte)((instruction >> 16) & 0xF);
            var rd = (byte)((instruction >> 12) & 0xF);
            var offset = instruction & 0xFFF;

            return new SingleDataTransferInstruction(position, condition, i, p, u, b, w, l, rn, rd, offset);
        }

        public void Execute(IEnvironment env)
        {
            var cpuState = env.CpuState as Aarch32CpuState;
            if (cpuState == null)
                throw new InvalidOperationException("Unknown cpu state.");
            var memoryMap = env.MemoryMap;

            if (!ConditionHelper.CanExecute(cpuState, _condition))
                return;

            var newOffset = _offset;
            if (_i)
                newOffset = ShiftValue(cpuState);

            var baseAddress = (long)cpuState.Registers[_rn];
            if (_p)
                if (_u)
                    baseAddress += newOffset;
                else
                    baseAddress -= newOffset;

            if (_l)
            {
                if (_b)
                    cpuState.Registers[_rd] = memoryMap.ReadByte((int)baseAddress);
                else
                    cpuState.Registers[_rd] = memoryMap.ReadUInt32((int)baseAddress);

                // TODO: Legacy code that called a method to reset the instruction queue to the new PC after it was changed
                //if (_rd == 0xF)
                //    cpuState.SetRegister("PC", cpuState.GetRegister($"R{_rd}"));
            }
            else
            {
                if (_b)
                    memoryMap.WriteByte((int)baseAddress, (byte)cpuState.Registers[_rd]);
                else
                    memoryMap.WriteUInt32((int)baseAddress, cpuState.Registers[_rd]);
            }

            if (!_p)
                if (_u)
                    baseAddress += newOffset;
                else
                    baseAddress -= newOffset;

            if (_w)
                cpuState.Registers[_rn] = (uint)baseAddress;
        }

        public override string ToString()
        {
            var shift = (_offset >> 4) & 0xFF;
            var shiftType = (shift >> 1) & 0x3;
            var shiftTypeName = Enum.GetName(typeof(ShiftType), shiftType);
            var shiftValue = ((_offset >> 4) & 0xFF) >> 3;
            if ((shift & 0x1) == 0 && (shiftType == 1 || shiftType == 2) && shiftValue == 0)
                shiftValue = 32;

            var result = _l ? "LDR" : "STR";
            if (_b)
                result += "B";
            if (_w)
                result += "T";
            result += ConditionHelper.ToString(_condition);
            result += " R" + _rd + ", [";
            result += "R" + _rn;
            if (!_p)
                result += "]";
            if (_offset != 0 || _i)
                if (!_i)
                    result += $", #0x{_offset:X}";
                else
                {
                    result += ", ";
                    result+=_u?"+":"-";
                    result += "R" + (_offset & 0xF);
                    if (shiftValue > 0)
                    {
                        result += ", ";
                        result += shiftTypeName;
                        result += "#" + shiftValue;
                    }
                }

            if (_p)
                result += "]";
            if (_w)
                result += "!";

            return result;
        }

        private uint ShiftValue(Aarch32CpuState cpuState)
        {
            var rm = _offset & 0xF;
            var shift = (_offset >> 4) & 0xFF;

            var shiftType = (shift >> 1) & 0x3;
            var shiftValue = shift >> 3;
            if ((shift & 0x1) == 0 && (shiftType == 1 || shiftType == 2) && shiftValue == 0)
                shiftValue = 32;

            var newOffset = BarrelShifter.Instance.ShiftByType((ShiftType)shiftType, cpuState.Registers[rm], (int)shiftValue, out var carry);
            cpuState.C = carry;

            return newOffset;
        }

        public void Dispose()
        {

        }
    }
}
