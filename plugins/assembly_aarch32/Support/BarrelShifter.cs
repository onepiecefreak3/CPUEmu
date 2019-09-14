using System;
using assembly_aarch32.Exceptions;
using assembly_aarch32.Models;

namespace assembly_aarch32.Support
{
    class BarrelShifter
    {
        private static readonly Lazy<BarrelShifter> Lazy = new Lazy<BarrelShifter>(() => new BarrelShifter());
        public static BarrelShifter Instance => Lazy.Value;

        public uint ShiftByType(ShiftType type, uint value, int count, out bool carry)
        {
            switch (type)
            {
                case ShiftType.Lsl:
                    return Lsl(value, count, out carry);

                case ShiftType.Lsr:
                    return Lsr(value, count, out carry);

                case ShiftType.Asr:
                    return Asr(value, count, out carry);

                case ShiftType.Ror:
                    return Ror(value, count, out carry);

                default:
                    // TODO: Check against ARM documentation.
                    throw new UnknownShiftTypeException((int)type);
            }
        }

        public uint Lsl(uint value, int count, out bool carry)
        {
            if (count >= 32)
            {
                if (count == 32)
                    carry = (value & 0x1) == 1;
                else
                    carry = false;

                return 0;
            }

            carry = ((value >> (32 - count)) & 0x1) == 1;
            return value << count;
        }

        public uint Lsr(uint value, int count, out bool carry)
        {
            if (count >= 32)
            {
                if (count == 32)
                    carry = ((value >> 31) & 0x1) == 1;
                else
                    carry = false;

                return 0;
            }

            carry = ((value >> (count - 1)) & 0x1) == 1;
            return value >> count;
        }

        public uint Asr(uint value, int count, out bool carry)
        {
            if (count >= 32)
            {
                var sign = (value >> 31) & 0x1;
                carry = sign == 1;

                if (sign == 1)
                    return 0xFFFFFFFF;
                else
                    return 0;
            }

            carry = (((int)value >> (count - 1)) & 0x1) == 1;
            return (uint)((int)value >> count);
        }

        public uint Ror(uint value, int count, out bool carry)
        {
            count &= 0x1F;
            carry = ((value >> (count - 1)) & 0x1) == 1;
            return (uint)((value >> count) | ((value & ((1 << count) - 1)) << (32 - count)));
        }
    }
}
