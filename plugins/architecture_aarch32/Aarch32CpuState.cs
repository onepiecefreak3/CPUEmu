using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CpuContract;
using CpuContract.Attributes;
using CpuContract.Memory;

namespace assembly_aarch32
{
    public class Aarch32CpuState : ICpuState
    {
        #region For internal use; Performant access

        internal bool Z { get; set; }
        internal bool C { get; set; }
        internal bool N { get; set; }
        internal bool V { get; set; }

        internal uint[] Registers { get; private set; }

        internal uint Sp
        {
            get => Registers[13];
            set => Registers[13] = value;
        }

        internal uint Lr
        {
            get => Registers[14];
            set => Registers[14] = value;
        }

        internal uint Pc
        {
            get => Registers[15];
            set => Registers[15] = value;
        }

        #endregion

        #region Constructors

        public Aarch32CpuState()
        {
            Registers = new uint[16];
        }

        #endregion

        public IDictionary<string, object> GetRegisters()
        {
            var result = new Dictionary<string, object>();

            for (var i = 0; i < 13; i++)
                result.Add("R" + i, Registers[i]);

            result.Add("SP", Sp);
            result.Add("LR", Lr);
            result.Add("PC", Pc);

            return result;
        }

        public object GetRegister(string register)
        {
            if (Regex.IsMatch(register, "R\\d+"))
                return Registers[Convert.ToInt32(register.Substring(1))];

            switch (register)
            {
                case "SP":
                    return Sp;
                case "LR":
                    return Lr;
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
                Registers[Convert.ToInt32(register.Substring(1))] = Convert.ToUInt32(value);
                return;
            }

            switch (register)
            {
                case "SP":
                    Registers[13] = Convert.ToUInt32(value);
                    break;
                case "LR":
                    Registers[14] = Convert.ToUInt32(value);
                    break;
                case "PC":
                    Registers[15] = Convert.ToUInt32(value);
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
                {"V", V}
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

                default:
                    throw new InvalidOperationException($"Flag {flag} is unknown.");
            }
        }

        public void Reset(IMemoryMap memoryMap)
        {
            Pc = (uint)memoryMap.Payload.Address;
            Sp = (uint)memoryMap.Stack.Address;

            for (var i = 0; i < 14; i++)
                Registers[i] = 0;

            C = N = V = Z = false;
        }

        public void Dispose()
        {
            Registers = null;
        }
    }
}
