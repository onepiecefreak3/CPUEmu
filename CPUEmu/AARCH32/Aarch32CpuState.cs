using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CPUEmu.Interfaces;

namespace CPUEmu.AARCH32
{
    class Aarch32CpuState : ICpuState
    {
        private bool _z;
        private bool _c;
        private bool _n;
        private bool _v;

        private readonly uint[] _regs;

        public Aarch32CpuState()
        {
            _regs = new uint[16];
        }

        private Aarch32CpuState(uint[] regs, bool z, bool c, bool n, bool v)
        {
            _regs = regs;
            _z = z;
            _c = c;
            _n = n;
            _v = v;
        }

        public IDictionary<string, object> GetRegisters()
        {
            var result = new Dictionary<string, object>();

            for (var i = 0; i < 13; i++)
                result.Add("R" + i, _regs[i]);

            result.Add("SP", _regs[13]);
            result.Add("LR", _regs[14]);
            result.Add("PC", _regs[15]);

            return result;
        }

        public object GetRegister(string register)
        {
            if (Regex.IsMatch(register, "R\\d+"))
                return _regs[Convert.ToInt32(register.Substring(1))];

            switch (register)
            {
                case "SP":
                    return _regs[13];
                case "LR":
                    return _regs[14];
                case "PC":
                    return _regs[15];
                default:
                    throw new InvalidOperationException($"Register {register} is unknown.");
            }
        }

        public void SetRegister(string register, object value)
        {
            if (Regex.IsMatch(register, "R\\d+"))
                _regs[Convert.ToInt32(register.Substring(1))] = Convert.ToUInt32(value);

            switch (register)
            {
                case "SP":
                    _regs[13] = Convert.ToUInt32(value);
                    break;
                case "LR":
                    _regs[14] = Convert.ToUInt32(value);
                    break;
                case "PC":
                    _regs[15] = Convert.ToUInt32(value);
                    break;
                default:
                    throw new InvalidOperationException($"Register {register} is unknown.");
            }
        }

        public IDictionary<string, object> GetFlags()
        {
            var result = new Dictionary<string, object>();

            result.Add("Z", _z ? 1 : 0);
            result.Add("C", _z ? 1 : 0);
            result.Add("N", _z ? 1 : 0);
            result.Add("V", _z ? 1 : 0);

            return result;
        }

        public object GetFlag(string flag)
        {
            switch (flag)
            {
                case "Z":
                    return _z;
                case "C":
                    return _c;
                case "N":
                    return _n;
                case "V":
                    return _v;
                default:
                    throw new InvalidOperationException($"Flag {flag} is unknown.");
            }
        }

        public void SetFlag(string flag, object value)
        {
            switch (flag)
            {
                case "Z":
                    _z = Convert.ToBoolean(value);
                    break;
                case "C":
                    _c = Convert.ToBoolean(value);
                    break;
                case "N":
                    _n = Convert.ToBoolean(value);
                    break;
                case "V":
                    _v = Convert.ToBoolean(value);
                    break;
                default:
                    throw new InvalidOperationException($"Flag {flag} is unknown.");
            }
        }

        public ICpuState Clone()
        {
            var newRegs = new uint[16];
            Array.Copy(newRegs, newRegs, 16);

            return new Aarch32CpuState(newRegs, _z, _c, _n, _v);
        }
    }
}
