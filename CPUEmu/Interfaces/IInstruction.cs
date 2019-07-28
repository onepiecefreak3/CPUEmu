using System;

namespace CPUEmu.Interfaces
{
    public interface IInstruction : IDisposable
    {
        int Position { get; }

        void Execute(IEnvironment env);
    }
}
