using System;
using System.Collections.Generic;
using System.Linq;
using CPUEmu.Interfaces;

namespace CPUEmu.Aarch32
{
    class Aarch32Executor : Executor
    {
        private Queue<IInstruction> _instructionBuffer;
        private Aarch32CpuState ArmCpuState => Environment.CpuState as Aarch32CpuState;

        public override IInstruction CurrentInstruction { get; set; }
        protected override bool IsFinished { get; set; }

        public Aarch32Executor(IList<IInstruction> instructions, IEnvironment environment) : base(instructions, environment)
        {
            ResetInternal();
        }

        protected override void SetCurrentInstruction()
        {
            CurrentInstruction = _instructionBuffer.Dequeue();
        }

        protected override void ExecuteInternal()
        {
            CurrentInstruction.Execute(Environment);

            if (CurrentInstruction.Position + 0x8 != ArmCpuState.PC)
            {
                _instructionBuffer.Clear();

                // Buffer next 2 instructions
                GetNextInstruction();
                GetNextInstruction();
            }
            else
            {
                GetNextInstruction();
            }

            IsFinished = !_instructionBuffer.Any();
        }

        protected override void ResetInternal()
        {
            _instructionBuffer = new Queue<IInstruction>(3);

            // Buffer first 2 instructions
            GetNextInstruction();
            GetNextInstruction();
        }

        private void GetNextInstruction()
        {
            switch (Environment.CpuState)
            {
                case Aarch32CpuState armCpuState:
                    if (armCpuState.PC + Environment.PayloadAddress >= 0 &&
                        armCpuState.PC + Environment.PayloadAddress < Environment.PayloadAddress + Instructions[Instructions.Count - 1].Position + 4)
                        _instructionBuffer.Enqueue(Instructions.First(x => x.Position == armCpuState.PC));
                    armCpuState.PC += 4;
                    break;
                default:
                    throw new InvalidOperationException("Unknown CpuState.");
            }
        }
    }
}
