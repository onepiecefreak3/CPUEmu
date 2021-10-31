using CpuContract;
using CpuContract.Attributes;
using CpuContract.Executor;
using Serilog;

namespace architecture_avr
{
    [UniqueIdentifier("Avr")]
    public class AvrArchitectureProvider : IArchitectureProvider
    {
        public IInstructionParser InstructionParser { get; }
        public IExecutor Executor { get; }

        public AvrArchitectureProvider(ILogger logger)
        {
            var instructionParser = new AvrInstructionParser();

            InstructionParser = instructionParser;
            Executor = new AvrExecutor(new AvrCpuState(), instructionParser, logger);
        }
    }
}
