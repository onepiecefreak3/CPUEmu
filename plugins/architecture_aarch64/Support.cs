using System;
using System.Collections.Generic;
using System.Text;

namespace assembly_aarch64
{
    class Support
    {
        internal static void SetFlagsArithmethic(Aarch64CpuState cpuState, ulong result, ulong value, ulong value2, bool add, bool carryIn, bool sf)
        {
            SetFlagsLogical(cpuState, result, sf);

            var bitDepth = sf ? 63 : 31;

            var c = !carryIn ? result < value : result <= value;
            var v = !add ?
                value >> bitDepth != value2 >> bitDepth && value >> bitDepth != result >> bitDepth :
                value >> bitDepth == value2 >> bitDepth && value >> bitDepth != result >> bitDepth;

            cpuState.C = c;
            cpuState.V = v;
        }

        internal static void SetFlagsLogical(Aarch64CpuState cpuState, ulong result, bool sf)
        {
            var z = result == 0;
            var n = result >> (sf ? 63 : 31) == 1;

            cpuState.Z = z;
            cpuState.N = n;
        }
    }
}
