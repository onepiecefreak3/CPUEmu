using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CpuContract.Memory;
using Serilog;

namespace CpuContract.Executor
{
    public abstract class BaseExecutor<TCpuState> : IExecutor
        where TCpuState : ICpuState
    {
        private readonly IExecutableInstructionParser<TCpuState> _instructionParser;

        private ConcurrentDictionary<IInstruction, bool> _breakPoints;
        private Thread _executionTask;

        #region Properties

        protected abstract bool IsFinished { get; set; }
        protected bool IsAborted { get; set; }

        protected TCpuState InternalCpuState { get; }
        protected ILogger Logger { get; }

        protected IList<IExecutableInstruction<TCpuState>> ExecutableInstructions => _instructionParser.Instructions;
        protected int CurrentInstructionIndex { get; set; }
        protected IExecutableInstruction<TCpuState> CurrentExecutableInstruction { get; set; }

        public bool IsHalted { get; private set; }

        public ICpuState CpuState => InternalCpuState;
        public IList<IInstruction> Instructions => ExecutableInstructions.Cast<IInstruction>().ToArray();
        public IInstruction CurrentInstruction => CurrentExecutableInstruction;

        #endregion

        #region Events

        public event EventHandler<InstructionExecuteEventArgs> InstructionExecuting;
        public event EventHandler<InstructionExecuteEventArgs> InstructionExecuted;
        public event EventHandler ExecutionStarted;
        public event EventHandler ExecutionFinished;
        public event EventHandler<InstructionExecuteEventArgs> ExecutionHalted;
        public event EventHandler<InstructionExecuteEventArgs> ExecutionAborted;
        public event EventHandler<InstructionExecuteEventArgs> BreakpointReached;
        public event EventHandler<InstructionExecuteEventArgs> ExecutionStepped;

        #endregion

        protected BaseExecutor(TCpuState cpuState, IExecutableInstructionParser<TCpuState> instructionParser, ILogger logger)
        {
            InternalCpuState = cpuState;
            Logger = logger;

            _instructionParser = instructionParser;
            _breakPoints = new ConcurrentDictionary<IInstruction, bool>();
        }

        public void ExecuteAsync(DeviceEnvironment environment, int waitMs = 0)
        {
            CurrentExecutableInstruction = null;
            _executionTask = new Thread(() => Execute(environment, waitMs));
            _executionTask.Start();
        }

        // Event invocation, break handling and so on
        private void Execute(DeviceEnvironment environment, int waitMs)
        {
            Reset(environment.MemoryMap);

            IsFinished = false;
            IsAborted = false;
            IsHalted = false;

            ExecutionStarted?.Invoke(this, new EventArgs());

            while (!IsFinished && !IsAborted)
            {
                // Get the instruction for the current cycle
                var currentInstruction = GetCurrentInstruction(environment);
                if (currentInstruction == null)
                {
                    IsFinished = true;
                    continue;
                }

                CurrentExecutableInstruction = currentInstruction;
                CurrentInstructionIndex = ExecutableInstructions.IndexOf(currentInstruction);

                // Check if instruction is marked as a breakpoint and if it is active
                if (_breakPoints.ContainsKey(CurrentInstruction) && _breakPoints[CurrentInstruction])
                {
                    BreakExecution(CurrentInstruction, ExecutableInstructions.IndexOf(CurrentExecutableInstruction));
                }
                else if (IsHalted)
                {
                    // If instruction is not a breakpoint and IsHalted was already true, we stepped one instruction further
                    ExecutionStepped?.Invoke(this, new InstructionExecuteEventArgs(CurrentInstruction, CurrentInstructionIndex));
                }

                // Halt the thread if a breakpoint was reached
                if (IsHalted)
                {
                    try
                    {
                        Thread.Sleep(Timeout.Infinite);
                    }
                    catch (ThreadInterruptedException)
                    {
                        // Through an interrupt, the thread will continue from here
                    }
                    catch (ThreadAbortException)
                    {
                        // Further execution will be aborted
                        IsAborted = true;
                        break;
                    }
                }

                // Execute instruction
                InstructionExecuting?.Invoke(this, new InstructionExecuteEventArgs(CurrentInstruction, CurrentInstructionIndex));

                if (waitMs > 0)
                    Thread.Sleep(waitMs);

                try
                {
                    CurrentExecutableInstruction.Execute(InternalCpuState, environment);
                    AfterInstructionExecute(environment);
                }
                catch (Exception e)
                {
                    // Abort execution when the instruction throws
                    Logger.Fatal("An exception occurred when executing an instruction.", e);
                    AbortExecution(true);

                    break;
                }

                InstructionExecuted?.Invoke(this, new InstructionExecuteEventArgs(CurrentInstruction, CurrentInstructionIndex));
            }

            if (!IsAborted)
                ExecutionFinished?.Invoke(this, new EventArgs());
        }

        protected abstract IExecutableInstruction<TCpuState> GetCurrentInstruction(DeviceEnvironment environment);

        // HINT: Should set IsFinished at some point
        protected abstract void AfterInstructionExecute(DeviceEnvironment environment);

        public abstract void Reset(IMemoryMap memoryMap);

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
            IsHalted = false;
            _executionTask.Interrupt();
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
