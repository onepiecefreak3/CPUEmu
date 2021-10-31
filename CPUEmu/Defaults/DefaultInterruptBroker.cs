using CpuContract;
using CpuContract.Attributes;
using Serilog;

namespace CPUEmu.Defaults
{
    [UniqueIdentifier("Default")]
    public class DefaultInterruptBroker : IInterruptBroker
    {
        private ILogger _logger;

        public DefaultInterruptBroker(ILogger logger)
        {
            _logger = logger;
        }

        public void Execute(int svc, DeviceEnvironment environment)
        {
            // Do nothing
            _logger?.Warning($"Svc {svc} stubbed.");
        }

        public void Dispose()
        {
            _logger = null;
        }
    }
}
