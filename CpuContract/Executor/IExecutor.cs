using System;
using System.Collections.Generic;
using CpuContract.Memory;

namespace CpuContract.Executor
{
    public interface IExecutor : IDisposable
    {
        #region Events

        event EventHandler<InstructionExecuteEventArgs> InstructionExecuting;
        event EventHandler<InstructionExecuteEventArgs> InstructionExecuted;
        event EventHandler ExecutionStarted;
        event EventHandler ExecutionFinished;
        event EventHandler<InstructionExecuteEventArgs> ExecutionHalted;
        event EventHandler<InstructionExecuteEventArgs> ExecutionAborted;
        event EventHandler<InstructionExecuteEventArgs> BreakpointReached;
        event EventHandler<InstructionExecuteEventArgs> ExecutionStepped;

        #endregion

        #region Properties

        ICpuState CpuState { get; }

        IInstruction CurrentInstruction { get; }

        IList<IInstruction> Instructions { get; }

        bool IsHalted { get; }

        #endregion

        void ExecuteAsync(DeviceEnvironment environment, int waitMs);
        void Reset(IMemoryMap memoryMap);

        #region Execution

        void HaltExecution();
        void ResumeExecution();
        void AbortExecution();
        void StepExecution();

        #endregion

        #region Breakpoints

        bool SetBreakpoint(IInstruction instructionToBreakOn);
        void DisableBreakpoint(IInstruction breakpointToDisable);
        void EnableBreakpoint(IInstruction breakpointToEnable);
        bool RemoveBreakpoint(IInstruction instructionToRemove);
        void ResetBreakpoints();

        IEnumerable<IInstruction> GetActiveBreakpoints();

        #endregion
    }
}
