using System.Collections.Generic;
using System.IO;
using CpuContract.Executor;

namespace CpuContract
{
    public interface IAssemblyAdapter
    {
        bool Identify(Stream assembly);

        IList<IInstruction> ParseAssembly(Stream assembly);

        IExecutionEnvironment CreateExecutionEnvironment(Stream assembly);

        IExecutor CreateExecutor();
    }
}
