using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CpuContract;
using CpuContract.Attributes;

namespace assembly_aarch32
{
    [UniqueIdentifier("Aarch32")]
    public class Aarch32CpuState : ICpuState
    {
        public bool Z { get; set; }
        public bool C { get; set; }
        public bool N { get; set; }
        public bool V { get; set; }

        public uint[] Registers { get; private set; }
        public uint SP
        {
            get => Registers[13];
            set => Registers[13] = value;
        }

        public uint LR
        {
            get => Registers[14];
            set => Registers[14] = value;
        }

        public uint PC
        {
            get => Registers[15];
            set => Registers[15] = value;
        }

        public Aarch32CpuState()
        {
            Registers = new uint[16];
        }

        private Aarch32CpuState(uint[] registers, bool z, bool c, bool n, bool v)
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

            for (var i = 0; i < 13; i++)
                result.Add("R" + i, Registers[i]);

            result.Add("SP", SP);
            result.Add("LR", LR);
            result.Add("PC", PC);

            return result;
        }

        public object GetRegister(string register)
        {
            if (Regex.IsMatch(register, "R\\d+"))
                return Registers[Convert.ToInt32(register.Substring(1))];

            switch (register)
            {
                case "SP":
                    return SP;
                case "LR":
                    return LR;
                case "PC":
                    return PC;
                default:
                    throw new InvalidOperationException($"Register {register} is unknown.");
            }
        }

        public void SetRegister(string register, object value)
        {
            if (Regex.IsMatch(register, "R\\d+"))
                Registers[Convert.ToInt32(register.Substring(1))] = Convert.ToUInt32(value);

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
            var result = new Dictionary<string, object>();

            result.Add("Z", Z);
            result.Add("C", C);
            result.Add("N", N);
            result.Add("V", V);

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

        public ICpuState Clone()
        {
            var newRegs = new uint[16];
            Array.Copy(Registers, newRegs, 16);

            return new Aarch32CpuState(newRegs, Z, C, N, V);
        }

        public void Dispose()
        {
            Registers = null;
        }
    }
}
