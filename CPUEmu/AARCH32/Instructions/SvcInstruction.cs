using CPUEmu.Interfaces;

namespace CPUEmu.Aarch32.Instructions
{
    class SvcInstruction : IInstruction
    {
        private readonly byte _condition;
        private readonly byte _svc;

        public int Position { get; }

        public SvcInstruction(int position, byte condition, int svc)
        {
            Position = position;

            _condition = condition;
            _svc = svc;
        }

        public void Execute(IEnvironment env)
        {
            if (!ConditionHelper.CanExecute(env.CpuState, _condition))
                return;

            env.InterruptBroker.Execute(_svc, env);
        }
    }
}
