using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuContract.Logging
{
    public interface ILogger:IDisposable
    {
        void Log(LogLevel logLevel, string message);
    }
}
