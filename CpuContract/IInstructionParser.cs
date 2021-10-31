using System;
using System.Collections.Generic;
using System.IO;

namespace CpuContract
{
    public interface IInstructionParser : IDisposable
    {
        void LoadPayload(Stream file, int baseAddress);
    }

    public interface IExecutableInstructionParser<TCpuState> : IInstructionParser
        where TCpuState : ICpuState
    {
        IList<IExecutableInstruction<TCpuState>> Instructions { get; }
    }
}
