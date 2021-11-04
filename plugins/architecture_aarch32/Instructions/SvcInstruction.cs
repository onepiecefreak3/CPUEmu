using assembly_aarch32.Support;
using CpuContract;

namespace assembly_aarch32.Instructions
{
    class SvcInstruction : IExecutableInstruction<Aarch32CpuState>
    {
        private readonly byte _condition;
        private readonly int _svc;

        public int Position { get; }

        public uint Length => 4;

        private SvcInstruction(int position, byte condition, int svc)
        {
            Position = position;

            _condition = condition;
            _svc = svc;
        }

        public static SvcInstruction Parse(int position, byte condition, uint instruction)
        {
            return new SvcInstruction(position, condition, (int)(instruction & 0xFFFFFF));
        }

        public void Execute(Aarch32CpuState cpuState, DeviceEnvironment env)
        {
            if (!ConditionHelper.CanExecute(cpuState, _condition))
                return;

            env.InterruptBroker.Execute(_svc, env);
        }

        public override string ToString()
        {
            return $"SVC {_svc}";
        }

        public void Dispose()
        {

        }
    }
}
