using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu.Interfaces
{
    public interface IArchitectureParser
    {
        IList<IInstruction> Parse(Stream assembly,IInterruptBroker broker);
    }
}
