using CPUEmu.Interfaces;

namespace CPUEmu.AARCH32.Instructions.DataProcessing
{
    class SvcInstruction : IInstruction
    {
        private readonly IInterruptBroker _broker;
        private readonly byte _svc;

        public int Position { get; }

        public SvcInstruction(int position, byte svc, IInterruptBroker broker)
        {
            Position = position;

            _svc = svc;
            _broker = broker;
        }

        public void Execute(ICpuState cpuState)
        {
            _broker.Execute(_svc, cpuState);
        }
    }
}
