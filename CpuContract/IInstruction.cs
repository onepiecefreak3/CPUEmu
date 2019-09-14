using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuContract
{
    public interface IInstruction : IDisposable
    {
        int Position { get; }

        void Execute(IExecutionEnvironment env);
    }
}
