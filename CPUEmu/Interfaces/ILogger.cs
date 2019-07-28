using System;

namespace CPUEmu.Interfaces
{
    public interface ILogger:IDisposable
    {
        void Log(string message);
    }
}
