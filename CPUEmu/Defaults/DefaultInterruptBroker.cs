using CPUEmu.Interfaces;

namespace CPUEmu.Defaults
{
    class DefaultInterruptBroker : IInterruptBroker
    {
        private ILogger _logger;

        public DefaultInterruptBroker(ILogger logger)
        {
            _logger = logger;
        }

        public void Execute(int svc, IEnvironment cpuState)
        {
            // Do nothing
        }

        public void Dispose()
        {
            _logger.Dispose();
            _logger = null;
        }
    }
}
