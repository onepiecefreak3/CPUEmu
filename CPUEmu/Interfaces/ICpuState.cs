using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu.Interfaces
{
    public interface ICpuState
    {
        IDictionary<string, object> GetRegisters();
        object GetRegister(string register);
        void SetRegister(string register, object value);

        IDictionary<string, object> GetFlags();
        object GetFlag(string flag);
        void SetFlag(string flag, object value);

        ICpuState Clone();
    }
}
