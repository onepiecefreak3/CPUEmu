using System;
using assembly_aarch32.Support;
using CpuContract;

namespace assembly_aarch32.Instructions
{
    class SvcInstruction : IInstruction
    {
        private readonly byte _condition;
        private readonly int _svc;

        public int Position { get; }

        private SvcInstruction(int position, byte condition, int svc)
        {
            Position = position;

            _condition = condition;
            _svc = svc;
        }

        public static IInstruction Parse(int position, byte condition, uint instruction)
        {
            return new SvcInstruction(position, condition, (int)(instruction & 0xFFFFFF));
        }

        public void Execute(IExecutionEnvironment env)
        {
            if (!(env.CpuState is Aarch32CpuState armCpuState))
                throw new InvalidOperationException("Unknown cpu state.");

            if (!ConditionHelper.CanExecute(armCpuState, _condition))
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
