using System;

namespace CpuContract
{
    public interface IInstruction : IDisposable
    {
        int Position { get; }

        uint Length { get; }
    }

    public interface IExecutableInstruction<in TCpuState> : IInstruction 
        where TCpuState : ICpuState
    {
        void Execute(TCpuState cpuState, DeviceEnvironment env);
    }
}
