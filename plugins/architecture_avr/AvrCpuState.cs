using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CpuContract;
using CpuContract.Memory;

namespace architecture_avr
{
    class AvrCpuState : ICpuState
    {
        #region For internal use; Performant access

        internal bool Z { get; set; }
        internal bool C { get; set; }
        internal bool N { get; set; }
        internal bool V { get; set; }
        internal bool S { get; set; }
        internal bool H { get; set; }
        internal bool T { get; set; }
        internal bool I { get; set; }

        internal IMemoryRegion Registers { get; private set; }

        internal ushort RegX
        {
            get => (ushort)(Registers[26] | (Registers[27] << 8));
            set
            {
                Registers[26] = (byte)value;
                Registers[27] = (byte)(value >> 8);
            }
        }

        internal ushort RegY
        {
            get => (ushort)(Registers[28] | (Registers[29] << 8));
            set
            {
                Registers[28] = (byte)value;
                Registers[29] = (byte)(value >> 8);
            }
        }

        internal ushort RegZ
        {
            get => (ushort)(Registers[30] | (Registers[31] << 8));
            set
            {
                Registers[30] = (byte)value;
                Registers[31] = (byte)(value >> 8);
            }
        }

        internal uint Sp { get; set; }

        // HINT: PC counts in words (2-byte values)
        internal uint Pc { get; set; }

        #endregion

        public IDictionary<string, object> GetRegisters()
        {
            var result = new Dictionary<string, object>();

            for (var i = 0; i < 32; i++)
                result.Add("R" + i, Registers?[i] ?? 0);

            result.Add("SP", Sp);
            result.Add("PC", Pc);

            return result;
        }

        public object GetRegister(string register)
        {
            if (Regex.IsMatch(register, "R\\d+"))
                return Registers?[Convert.ToInt32(register.Substring(1))] ?? 0;

            switch (register)
            {
                case "SP":
                    return Sp;
                case "PC":
                    return Pc;
                default:
                    throw new InvalidOperationException($"Register {register} is unknown.");
            }
        }

        public void SetRegister(string register, object value)
        {
            if (Regex.IsMatch(register, "R\\d+"))
            {
                if (Registers == null)
                    return;

                Registers[Convert.ToInt32(register.Substring(1))] = Convert.ToByte(value);
                return;
            }

            switch (register)
            {
                case "SP":
                    Sp = Convert.ToUInt16(value);
                    break;
                case "PC":
                    Pc = Convert.ToUInt32(value);
                    break;
                default:
                    throw new InvalidOperationException($"Register {register} is unknown.");
            }
        }

        public IDictionary<string, object> GetFlags()
        {
            var result = new Dictionary<string, object>
            {
                {"Z", Z},
                {"C", C},
                {"N", N},
                {"V", V},
                {"H", H},
                {"T", T},
                {"I", I}
            };

            return result;
        }

        public object GetFlag(string flag)
        {
            switch (flag)
            {
                case "Z":
                    return Z;

                case "C":
                    return C;

                case "N":
                    return N;

                case "V":
                    return V;

                case "H":
                    return H;

                case "T":
                    return T;

                case "I":
                    return I;

                default:
                    throw new InvalidOperationException($"Flag {flag} is unknown.");
            }
        }

        public void SetFlag(string flag, object value)
        {
            switch (flag)
            {
                case "Z":
                    Z = Convert.ToBoolean(value);
                    break;

                case "C":
                    C = Convert.ToBoolean(value);
                    break;

                case "N":
                    N = Convert.ToBoolean(value);
                    break;

                case "V":
                    V = Convert.ToBoolean(value);
                    break;

                case "H":
                    H = Convert.ToBoolean(value);
                    break;

                case "T":
                    T = Convert.ToBoolean(value);
                    break;

                case "I":
                    I = Convert.ToBoolean(value);
                    break;

                default:
                    throw new InvalidOperationException($"Flag {flag} is unknown.");
            }
        }

        public void Reset(IMemoryMap memoryMap)
        {
            Pc = (uint)memoryMap.Payload.Address / 2;

            // Stack gets read from end to start, so start stack at the end of its range
            Sp = (uint)(memoryMap.Stack.Address + memoryMap.Stack.Length - 1);

            // Registers and IO are mapped at the start of the memory
            Registers = memoryMap.GetRegion(0, 0x20);
            Registers.ClearAll();

            C = N = V = Z = H = T = I = false;
        }

        public void Dispose()
        {
            Registers = null;
        }
    }
}
