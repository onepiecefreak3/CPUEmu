using System;
using System.Collections.Generic;

namespace CpuContract.Executor
{
    // TODO: Split into smaller interfaces
    public interface IExecutor : IDisposable
    {
        event EventHandler<InstructionExecuteEventArgs> InstructionExecuting;
        event EventHandler<InstructionExecuteEventArgs> InstructionExecuted;
        event EventHandler ExecutionStarted;
        event EventHandler ExecutionFinished;
        event EventHandler<InstructionExecuteEventArgs> ExecutionHalted;
        event EventHandler<InstructionExecuteEventArgs> ExecutionAborted;
        event EventHandler<InstructionExecuteEventArgs> BreakpointReached;
        event EventHandler<InstructionExecuteEventArgs> ExecutionStepped;

        IInstruction CurrentInstruction { get; }

        bool IsHalted { get; }

        void ExecuteAsync(IExecutionEnvironment environment, IList<IInstruction> instructions, int waitMs);
        void Reset();

        void HaltExecution();
        void ResumeExecution();
        void AbortExecution();
        void StepExecution();

        bool SetBreakpoint(IInstruction instructionToBreakOn);
        void DisableBreakpoint(IInstruction breakpointToDisable);
        void EnableBreakpoint(IInstruction breakpointToEnable);
        bool RemoveBreakpoint(IInstruction instructionToRemove);
        void ResetBreakpoints();
        IEnumerable<IInstruction> GetActiveBreakpoints();
    }
}
