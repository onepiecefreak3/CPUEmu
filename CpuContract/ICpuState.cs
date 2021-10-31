using System;
using System.Collections.Generic;
using CpuContract.Memory;

namespace CpuContract
{
    public interface ICpuState : IDisposable
    {
        IDictionary<string, object> GetRegisters();
        object GetRegister(string register);
        void SetRegister(string register, object value);

        IDictionary<string, object> GetFlags();
        object GetFlag(string flag);
        void SetFlag(string flag, object value);

        void Reset(IMemoryMap memoryMap);
    }
}
