using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CpuContract.Logging;

namespace CpuContract.Executor
{
    public abstract class BaseExecutor : IExecutor
    {
        private ConcurrentDictionary<IInstruction, bool> _breakPoints;
        private IInstruction _breakPointInstruction;

        private Thread _executionTask;

        protected abstract bool IsFinished { get; set; }
        protected bool IsAborted { get; set; }
        protected ILogger Logger { get; }

        public bool IsHalted { get; private set; }

        public abstract IInstruction CurrentInstruction { get; protected set; }
        protected abstract int CurrentInstructionIndex { get; set; }

        public event EventHandler<InstructionExecuteEventArgs> InstructionExecuting;
        public event EventHandler<InstructionExecuteEventArgs> InstructionExecuted;
        public event EventHandler ExecutionStarted;
        public event EventHandler ExecutionFinished;
        public event EventHandler<InstructionExecuteEventArgs> ExecutionHalted;
        public event EventHandler<InstructionExecuteEventArgs> ExecutionAborted;
        public event EventHandler<InstructionExecuteEventArgs> BreakpointReached;

        protected BaseExecutor(ILogger logger)
        {
            Logger = logger;
            _breakPoints = new ConcurrentDictionary<IInstruction, bool>();
        }

        public void ExecuteAsync(IExecutionEnvironment environment, IList<IInstruction> instructions, int waitMs = 0)
        {
            CurrentInstruction = null;
            _executionTask = new Thread(() => Execute(environment, instructions, waitMs));
            _executionTask.Start();
        }

        // Event invocation, break handling and so on
        private void Execute(IExecutionEnvironment environment, IList<IInstruction> instructions, int waitMs)
        {
            Reset();
            IsFinished = false;
            IsAborted = false;
            IsHalted = false;

            ExecutionStarted?.Invoke(this, new EventArgs());

            while (!IsFinished && !IsAborted)
            {
                if (IsHalted)
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

                SetCurrentInstruction(environment, instructions);
                if (_breakPointInstruction != CurrentInstruction && _breakPoints.ContainsKey(CurrentInstruction) && _breakPoints[CurrentInstruction])
                {
                    BreakExecution(CurrentInstruction, instructions.IndexOf(CurrentInstruction));
                    _breakPointInstruction = CurrentInstruction;
                    continue;
                }

                InstructionExecuting?.Invoke(this, new InstructionExecuteEventArgs(CurrentInstruction, instructions.IndexOf(CurrentInstruction)));

                Thread.Sleep(waitMs);
                try
                {
                    ExecuteInternal(environment, instructions);
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Fatal, e.Message);
                    AbortExecution(true);
                    break;
                }

                InstructionExecuted?.Invoke(this, new InstructionExecuteEventArgs(CurrentInstruction, instructions.IndexOf(CurrentInstruction)));
            }

            if (!IsAborted)
                ExecutionFinished?.Invoke(this, new EventArgs());
        }

        protected abstract void SetCurrentInstruction(IExecutionEnvironment environment, IList<IInstruction> instructions);

        // Should set IsFinished at some point
        protected abstract void ExecuteInternal(IExecutionEnvironment environment, IList<IInstruction> instructions);

        public abstract void Reset();

        public void HaltExecution()
        {
            IsHalted = true;
            ExecutionHalted?.Invoke(this, new InstructionExecuteEventArgs(CurrentInstruction, CurrentInstructionIndex));
        }

        private void BreakExecution(IInstruction instruction, int index)
        {
            IsHalted = true;
            BreakpointReached?.Invoke(this, new InstructionExecuteEventArgs(instruction, index));
        }

        public void ResumeExecution()
        {
            _executionTask.Interrupt();
            IsHalted = false;
        }

        public void AbortExecution()
        {
            AbortExecution(true);
            AbortThread();
        }

        public void StepExecution()
        {
            _executionTask.Interrupt();
        }

        protected void AbortExecution(bool invokeEvent)
        {
            IsAborted = true;
            _breakPointInstruction = null;
            if (invokeEvent)
                ExecutionAborted?.Invoke(this, new InstructionExecuteEventArgs(CurrentInstruction, CurrentInstructionIndex));
        }

        protected void AbortThread()
        {
            _executionTask?.Abort();
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
            AbortThread();
            _executionTask = null;
        }
    }
}
