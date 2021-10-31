using System;
using System.Collections.Generic;
using System.Linq;
using assembly_aarch32.Instructions.Branch;
using CpuContract;
using CpuContract.Executor;
using CpuContract.Memory;
using Serilog;

namespace assembly_aarch32
{
    public class Aarch32Executor : BaseExecutor<Aarch32CpuState>
    {
        private readonly Queue<IExecutableInstruction<Aarch32CpuState>> _instructionBuffer;

        protected override bool IsFinished { get; set; }

        public Aarch32Executor(Aarch32CpuState cpuState, IExecutableInstructionParser<Aarch32CpuState> instructionParser, ILogger logger) :
            base(cpuState, instructionParser, logger)
        {
            _instructionBuffer = new Queue<IExecutableInstruction<Aarch32CpuState>>(3);
        }

        protected override IExecutableInstruction<Aarch32CpuState> GetCurrentInstruction(DeviceEnvironment environment)
        {
            if (CurrentExecutableInstruction == null)
            {
                // Should buffer the first 2 instructions and then never pass here again in one execution run
                GetNextInstruction(environment);
                GetNextInstruction(environment);
            }

            return _instructionBuffer.Count == 0 ? null : _instructionBuffer.Dequeue();
        }

        protected override void AfterInstructionExecute(DeviceEnvironment environment)
        {
            if (CurrentExecutableInstruction is BranchAndExchangeInstruction baeInstruction)
                if (baeInstruction.IsThumbMode)
                    throw new InvalidOperationException("Thumb mode is not supported.");

            // If a branch was executed
            if (CurrentExecutableInstruction is BranchInstruction branchInstruction && branchInstruction.IsBranching)
            {
                // Clear buffer and and get 2 new instructions from new PC
                _instructionBuffer.Clear();

                // Buffer next 2 instructions
                GetNextInstruction(environment);
                GetNextInstruction(environment);
            }
            else
            {
                // Otherwise buffer the next instruction
                GetNextInstruction(environment);
            }

            IsFinished = !_instructionBuffer.Any();
        }

        public override void Reset(IMemoryMap memoryMap)
        {
            CurrentInstructionIndex = 0;
            CurrentExecutableInstruction = null;

            _instructionBuffer.Clear();

            InternalCpuState.Reset(memoryMap);
        }

        private void GetNextInstruction(DeviceEnvironment environment)
        {
            var absolutePcAddress = environment.MemoryMap.Payload.Address + InternalCpuState.Pc;
            var maxInstructionAddress = environment.MemoryMap.Payload.Address + environment.MemoryMap.Payload.Length;

            var instructionLength = 0u;
            if (absolutePcAddress >= 0 && absolutePcAddress < maxInstructionAddress)
            {
                var nextInstruction = ExecutableInstructions.FirstOrDefault(x => x.Position == InternalCpuState.Pc);
                if (nextInstruction != null)
                {
                    _instructionBuffer.Enqueue(nextInstruction);
                    instructionLength = (uint)nextInstruction.Length;
                }
            }

            InternalCpuState.Pc += instructionLength;
        }
    }
}
