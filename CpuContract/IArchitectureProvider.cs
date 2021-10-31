using CpuContract.Executor;

namespace CpuContract
{
    public interface IArchitectureProvider
    {
        IInstructionParser InstructionParser { get; }

        IExecutor Executor { get; }
    }
}
