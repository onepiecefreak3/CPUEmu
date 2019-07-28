using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CPUEmu.Interfaces;

namespace CPUEmu
{
    public abstract class Executor : IDisposable
    {
        protected abstract bool IsFinished { get; set; }
        protected bool IsAborted { get; set; }

        protected IList<IInstruction> Instructions { get; }
        protected IEnvironment Environment { get; }

        public bool IsHalted { get; private set; }

        public abstract IInstruction CurrentInstruction { get; set; }

        public event EventHandler InstructionExecuting;
        public event EventHandler InstructionExecuted;
        public event EventHandler ExecutionFinished;
        public event EventHandler ExecutionAborted;

        protected Executor(IList<IInstruction> instructions, IEnvironment environment)
        {
            Instructions = instructions;
            Environment = environment;
        }

        public void ExecuteAsync()
        {
            Task.Factory.StartNew(Execute);
        }

        // TODO: Breakpoint handling
        // Event invocation and so on
        private void Execute()
        {
            Reset();
            IsFinished = false;
            IsAborted = false;

            while (!IsFinished && !IsAborted)
            {
                if (!IsHalted)
                {
                    InstructionExecuting?.Invoke(this, new EventArgs());

                    ExecuteInternal();

                    InstructionExecuted?.Invoke(this, new EventArgs());
                }
            }

            if (IsAborted)
                ExecutionAborted?.Invoke(this, new EventArgs());
            else
                ExecutionFinished?.Invoke(this, new EventArgs());
        }

        // Should set IsFinished at some point
        protected abstract void ExecuteInternal();

        private void Reset()
        {
            Environment.Reset();
            ResetInternal();
        }

        protected abstract void ResetInternal();

        public void HaltExecution()
        {
            IsHalted = true;
        }

        public void ResumeExecution()
        {
            IsHalted = false;
        }

        public void AbortExecution()
        {
            IsAborted = true;
        }

        public void Dispose()
        {
            foreach (var inst in Instructions)
                inst.Dispose();
            Environment.Dispose();
        }
    }
}
