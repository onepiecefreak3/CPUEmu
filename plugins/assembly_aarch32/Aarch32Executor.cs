﻿using System;
using System.Collections.Generic;
using System.Linq;
using CpuContract;
using CpuContract.Attributes;
using CpuContract.Executor;
using CpuContract.Logging;

namespace assembly_aarch32
{
    [UniqueIdentifier("Aarch32")]
    public class Aarch32Executor : BaseExecutor
    {
        private Queue<IInstruction> _instructionBuffer;

        public override IInstruction CurrentInstruction { get; protected set; }
        protected override int CurrentInstructionIndex { get; set; }
        protected override bool IsFinished { get; set; }

        public Aarch32Executor(ILogger logger) : base(logger)
        {
            _instructionBuffer = new Queue<IInstruction>(3);
        }

        // TODO: Rethink design to maybe not pass the instruction lists
        protected override void SetCurrentInstruction(IExecutionEnvironment environment, IList<IInstruction> instructions)
        {
            if (CurrentInstruction == null)
            {
                // Should buffer the first 2 instructions and then never pass here again in one execution run
                GetNextInstruction(environment, instructions);
                GetNextInstruction(environment, instructions);
            }

            CurrentInstruction = _instructionBuffer.Dequeue();
        }

        protected override void ExecuteInternal(IExecutionEnvironment environment, IList<IInstruction> instructions)
        {
            CurrentInstruction.Execute(environment);

            if (!(environment.CpuState is Aarch32CpuState armCpuState))
                throw new InvalidOperationException("CpuState is not supported.");

            if (CurrentInstruction.Position + 0x8 != armCpuState.PC)
            {
                _instructionBuffer.Clear();

                // Buffer next 2 instructions
                GetNextInstruction(environment, instructions);
                GetNextInstruction(environment, instructions);
            }
            else
            {
                GetNextInstruction(environment, instructions);
            }

            IsFinished = !_instructionBuffer.Any();
        }

        public override void Reset()
        {
            CurrentInstructionIndex = 0;
            CurrentInstruction = null;

            _instructionBuffer.Clear();
        }

        private void GetNextInstruction(IExecutionEnvironment environment, IList<IInstruction> instructions)
        {
            switch (environment.CpuState)
            {
                case Aarch32CpuState armCpuState:
                    if (armCpuState.PC + environment.MemoryMap.Payload.Address >= 0 &&
                        armCpuState.PC + environment.MemoryMap.Payload.Address < environment.MemoryMap.Payload.Address + instructions[instructions.Count - 1].Position + 4)
                        _instructionBuffer.Enqueue(instructions.First(x => x.Position == armCpuState.PC));
                    armCpuState.PC += 4;
                    break;
                default:
                    throw new InvalidOperationException("Unknown CpuState.");
            }
        }
    }
}