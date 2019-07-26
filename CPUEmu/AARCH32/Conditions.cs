using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPUEmu.Interfaces;

namespace CPUEmu.AARCH32
{
    class ConditionHelper
    {
        public static bool CanExecute(ICpuState state, byte condition)
        {
            var (z, c, n, v) = (Convert.ToBoolean(state.GetFlag("Z")),
                Convert.ToBoolean(state.GetFlag("C")),
                Convert.ToBoolean(state.GetFlag("N")),
                Convert.ToBoolean(state.GetFlag("V")));

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
                    Log?.Invoke(this, $"Unknown condition 0x{condition:X1}. Ignore instruction.");
                    return false;
            }
        }

        private static Dictionary<byte, string> _condNames = new Dictionary<byte, string>
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
            return _condNames[condition];
        }
    }
}
