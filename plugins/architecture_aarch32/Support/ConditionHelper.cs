using System;
using System.Collections.Generic;
using assembly_aarch32.Exceptions;
using CpuContract;

namespace assembly_aarch32.Support
{
    class ConditionHelper
    {
        public static bool CanExecute(Aarch32CpuState state, byte condition)
        {
            if (condition > 14)
                throw new UnknownConditionException(condition);

            var (z, c, n, v) = (state.Z, state.C, state.N, state.V);

            switch (condition)
            {
                case 0:
                    return z;
                case 1:
                    return !z;
                case 2:
                    return c;
                case 3:
                    return !c;
                case 4:
                    return n;
                case 5:
                    return !n;
                case 6:
                    return v;
                case 7:
                    return !v;
                case 8:
                    return c && !z;
                case 9:
                    return !c || z;
                case 10:
                    return n == v;
                case 11:
                    return n != v;
                case 12:
                    return !z && n == v;
                case 13:
                    return z || n != v;
                case 14:
                    return true;
                default:
                    // Path gets never reached, due to initial if statement
                    return false;
            }
        }

        private static readonly Dictionary<byte, string> CondNames = new Dictionary<byte, string>
        {
            [0] = "EQ",
            [1] = "NE",
            [2] = "CS",
            [3] = "CC",
            [4] = "MI",
            [5] = "PL",
            [6] = "VS",
            [7] = "VC",
            [8] = "HI",
            [9] = "LS",
            [10] = "GE",
            [11] = "LT",
            [12] = "GT",
            [13] = "LE",
            [14] = "",
            [15] = ""
        };

        public static string ToString(byte condition)
        {
            if (!CondNames.ContainsKey(condition))
                throw new UnknownConditionException(condition);

            return CondNames[condition];
        }
    }
}
