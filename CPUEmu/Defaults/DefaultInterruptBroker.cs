using CpuContract;
using CpuContract.Attributes;
using CpuContract.Logging;

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

        public void Execute(int svc, IExecutionEnvironment environment)
        {
            // Do nothing
            _logger.Log(LogLevel.Warning, $"Svc {svc} stubbed.");
        }

        public void Dispose()
        {
            _logger.Dispose();
            _logger = null;
        }
    }
}
