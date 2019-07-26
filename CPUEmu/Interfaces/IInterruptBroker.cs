using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu.Interfaces
{
    public interface IInterruptBroker
    {
        void Execute(int svc, ICpuState cpuState);
    }
}
