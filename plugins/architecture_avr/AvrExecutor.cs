using System.Linq;
using architecture_avr.Instructions.Branch;
using CpuContract;
using CpuContract.Executor;
using CpuContract.Memory;
using Serilog;

namespace architecture_avr
{
    class AvrExecutor : BaseExecutor<AvrCpuState>
    {
        protected override bool IsFinished { get; set; }

        public AvrExecutor(AvrCpuState cpuState, IExecutableInstructionParser<AvrCpuState> instructionParser, ILogger logger) :
            base(cpuState, instructionParser, logger)
        {
        }

        protected override IExecutableInstruction<AvrCpuState> GetCurrentInstruction(DeviceEnvironment environment)
        {
            return GetInstructionAt(InternalCpuState.Pc);
        }

        protected override void AfterInstructionExecute(DeviceEnvironment environment)
        {
            if (CurrentExecutableInstruction == null)
            {
                IsFinished = true;
                return;
            }

            // After instruction execution, increase PC
            InternalCpuState.Pc += CurrentExecutableInstruction.Length / 2;

            // A CPSE instruction skips the next 1- or 2-word instruction
            if (CurrentExecutableInstruction is CpseInstruction cpse)
                if (cpse.IsSkipping)
                    InternalCpuState.Pc += GetInstructionAt(InternalCpuState.Pc)?.Length / 2 ?? 0;
        }

        public override void Reset(IMemoryMap memoryMap)
        {
            InternalCpuState.Reset(memoryMap);
        }

        private IExecutableInstruction<AvrCpuState> GetInstructionAt(uint pc)
        {
            return ExecutableInstructions.FirstOrDefault(x => x.Position == pc * 2);
        }
    }
}
