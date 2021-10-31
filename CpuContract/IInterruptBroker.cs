using System;

namespace CpuContract
{
    public interface IInterruptBroker : IDisposable
    {
        void Execute(int svc, DeviceEnvironment environment);
    }
}
