using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu
{
    class IndexEventArgs : EventArgs
    {
        public int AbsoluteIndex { get; }

        public IndexEventArgs(int absoluteIndex)
        {
            AbsoluteIndex = absoluteIndex;
        }
    }
}
