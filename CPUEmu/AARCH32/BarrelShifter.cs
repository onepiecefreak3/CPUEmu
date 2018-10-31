﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu
{
    internal class BarrelShifter
    {
        public enum ShiftType : uint
        {
            LSL,
            LSR,
            ASR,
            ROR
        }

        public uint ShiftByType(ShiftType type, uint value, int count, out bool carry)
        {
            switch (type)
            {
                case ShiftType.LSL:
                    return LSL(value, count, out carry);

                case ShiftType.LSR:
                    return LSR(value, count, out carry);

                case ShiftType.ASR:
                    return ASR(value, count, out carry);

                case ShiftType.ROR:
                    return ROR(value, count, out carry);

                default:
                    carry = false;
                    return 0;
            }
        }

        public uint LSL(uint value, int count, out bool carry)
        {
            count &= 0x1F;
            carry = ((value >> (32 - count)) & 0x1) == 1;
            return value << count;
        }

        public uint LSR(uint value, int count, out bool carry)
        {
            count &= 0x1F;
            carry = ((value >> (count - 1)) & 0x1) == 1;
            return value >> count;
        }

        public uint ASR(uint value, int count, out bool carry)
        {
            count &= 0x1F;
            carry = (((int)value >> (count - 1)) & 0x1) == 1;
            return (uint)((int)value >> count);
        }

        public uint ROR(uint value, int count, out bool carry)
        {
            count &= 0x1F;
            carry = ((value >> (count - 1)) & 0x1) == 1;
            return (uint)((value >> count) | ((value & ((1 << count) - 1)) << (32 - count)));
        }
    }
}
