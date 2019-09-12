using System;

namespace CPUEmu.Interfaces
{
    public interface ILogger : IDisposable
    {
        void Log(LogLevel logLevel, string message);
    }
}
