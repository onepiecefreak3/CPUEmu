using CpuContract;
using CpuContract.Attributes;
using CpuContract.Executor;
using Serilog;

namespace assembly_aarch32
{
    [UniqueIdentifier("Aarch32")]
    public class Aarch32ArchitectureProvider : IArchitectureProvider
    {
        public IInstructionParser InstructionParser { get; }
        public IExecutor Executor { get; }

        public Aarch32ArchitectureProvider(ILogger logger)
        {
            var parser = new Aarch32InstructionParser(logger);

            InstructionParser = parser;
            Executor = new Aarch32Executor(new Aarch32CpuState(), parser, logger);
        }
    }
}
