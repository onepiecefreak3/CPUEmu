using System;

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
