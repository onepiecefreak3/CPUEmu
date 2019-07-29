using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPUEmu.Interfaces;

namespace CPUEmu
{
    public abstract class Executor : IDisposable
    {
        private ConcurrentDictionary<IInstruction, bool> _breakPoints;
        private IInstruction _breakPointInstruction;

        private bool _doStep;
        private Thread _executionTask;

        protected abstract bool IsFinished { get; set; }
        protected bool IsAborted { get; set; }

        protected IList<IInstruction> Instructions { get; }
        protected IEnvironment Environment { get; }

        public bool IsHalted { get; private set; }

        public abstract IInstruction CurrentInstruction { get; set; }

        public event EventHandler InstructionExecuting;
        public event EventHandler InstructionExecuted;
        public event EventHandler ExecutionStarted;
        public event EventHandler ExecutionFinished;
        public event EventHandler ExecutionHalted;
        public event EventHandler ExecutionAborted;
        public event EventHandler BreakpointReached;

        protected Executor(IList<IInstruction> instructions, IEnvironment environment)
        {
            Instructions = instructions;
            Environment = environment;
            _breakPoints = new ConcurrentDictionary<IInstruction, bool>();
        }

        public void ExecuteAsync(int waitMs = 0)
        {
            _executionTask = new Thread(() => Execute(waitMs));
            _executionTask.Start();
        }

        // Event invocation, break handling and so on
        private void Execute(int waitMs)
        {
            Reset();
            IsFinished = false;
            IsAborted = false;
            IsHalted = false;

            ExecutionStarted?.Invoke(this, new EventArgs());

            while (!IsFinished && !IsAborted)
            {
                if (IsHalted && !_doStep)
                {
                    try
                    {
                        Thread.Sleep(Timeout.Infinite);
                    }
                    catch (ThreadInterruptedException)
                    {
                        // Through the interrupt in ResumeExecution, the thread will continue from here
                    }
                    catch (ThreadAbortException)
                    {
                        IsAborted = true;
                        break;
                    }
                }


                if (_doStep)
                    _doStep = false;

                if (CurrentInstruction == null)
                    SetCurrentInstruction();
                if (_breakPointInstruction != CurrentInstruction && _breakPoints.ContainsKey(CurrentInstruction) && _breakPoints[CurrentInstruction])
                {
                    BreakExecution();
                    _breakPointInstruction = CurrentInstruction;
                    continue;
                }

                InstructionExecuting?.Invoke(this, new EventArgs());

                Thread.Sleep(waitMs);
                ExecuteInternal();
                CurrentInstruction = null;

                InstructionExecuted?.Invoke(this, new EventArgs());
            }

            if (!IsAborted)
                ExecutionFinished?.Invoke(this, new EventArgs());
        }

        protected abstract void SetCurrentInstruction();

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
            ExecutionHalted?.Invoke(this, new EventArgs());
        }

        private void BreakExecution()
        {
            IsHalted = true;
            BreakpointReached?.Invoke(this, new EventArgs());
        }

        public void ResumeExecution()
        {
            _executionTask.Interrupt();
            IsHalted = false;
        }

        public void AbortExecution()
        {
            AbortExecution(true);
        }

        protected void AbortExecution(bool invokeEvent)
        {
            IsAborted = true;
            _executionTask?.Abort();
            _breakPointInstruction = null;
            if (invokeEvent)
                ExecutionAborted?.Invoke(this, new EventArgs());
        }

        public bool SetBreakpoint(IInstruction instructionToBreakOn)
        {
            return _breakPoints.TryAdd(instructionToBreakOn, true);
        }

        public void DisableBreakpoint(IInstruction breakpointToDisable)
        {
            _breakPoints[breakpointToDisable] = false;
        }

        public void EnableBreakpoint(IInstruction breakpointToEnable)
        {
            _breakPoints[breakpointToEnable] = true;
        }

        public bool RemoveBreakpoint(IInstruction instructionToRemove)
        {
            return _breakPoints.TryRemove(instructionToRemove, out _);
        }

        public void ResetBreakpoints()
        {
            _breakPoints = new ConcurrentDictionary<IInstruction, bool>();
        }

        public IEnumerable<IInstruction> GetActiveBreakpoints()
        {
            return _breakPoints.Where(x => x.Value).Select(x => x.Key);
        }

        public void Dispose()
        {
            AbortExecution(false);
            foreach (var inst in Instructions)
                inst.Dispose();
            Environment.Dispose();
            _executionTask = null;
        }
    }
}
