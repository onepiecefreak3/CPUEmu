using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CpuContract;

namespace assembly_aarch64
{
    public class Aarch64CpuState : ICpuState
    {
        public bool Z { get; set; }
        public bool C { get; set; }
        public bool N { get; set; }
        public bool V { get; set; }

        public ulong[] Registers { get; private set; }
        public ulong Lr
        {
            get => Registers[30];
            set => Registers[30] = value;
        }
        public ulong Sp
        {
            get => Registers[31];
            set => Registers[31] = value;
        }

        public ulong Pc
        {
            get => Registers[32];
            set => Registers[32] = value;
        }

        public Aarch64CpuState()
        {
            Registers = new ulong[33];
        }

        public Aarch64CpuState(ulong[] registers, bool z, bool c, bool n, bool v)
        {
            Registers = registers;
            Z = z;
            C = c;
            N = n;
            V = v;
        }

        public IDictionary<string, object> GetRegisters()
        {
            var result = new Dictionary<string, object>();

            for (var i = 0; i < 30; i++)
                result.Add("R" + i, Registers[i]);

            result.Add("LR", Lr);
            result.Add("SP", Sp);
            result.Add("PC", Pc);

            return result;
        }

        public object GetRegister(string register)
        {
            if (Regex.IsMatch(register, "R\\d+"))
                return Registers[Convert.ToInt32(register.Substring(1))];

            switch (register)
            {
                case "LR":
                    return Registers[30];
                case "SP":
                    return Registers[31];
                case "PC":
                    return Registers[32];
                default:
                    throw new InvalidOperationException($"Register {register} is unknown.");
            }
        }

        public void SetRegister(string register, object value)
        {
            if (Regex.IsMatch(register, "R\\d+"))
            {
                Registers[Convert.ToUInt32(register.Substring(1))] = Convert.ToUInt64(value);
                return;
            }

            switch (register)
            {
                case "LR":
                    Registers[30] = Convert.ToUInt64(value);
                    break;
                case "SP":
                    Registers[31] = Convert.ToUInt64(value);
                    break;
                case "PC":
                    Registers[32] = Convert.ToUInt64(value);
                    break;
                default:
                    throw new InvalidOperationException($"Register {register} is unknown.");
            }
        }

        public IDictionary<string, object> GetFlags()
        {
            var result = new Dictionary<string, object>
            {
                { "Z", Z }, 
                { "C", C }, 
                { "N", N }, 
                { "V", V }
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
    }
}
