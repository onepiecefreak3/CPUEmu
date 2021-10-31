using CpuContract;
using CpuContract.Memory;

namespace CPUEmu.MemoryManipulation
{
    public class ValueMemoryManipulation : IMemoryManipulation
    {
        private byte _value;

        public int Offset { get; }

        public ValueMemoryManipulation(int offset, byte value)
        {
            Offset = offset;
            _value = value;
        }

        public void Execute(IMemoryMap memoryMapMap)
        {
            memoryMapMap.WriteByte(Offset, _value);
        }

        public override string ToString()
        {
            return $"Set value '0x{_value:X2}' at offset '0x{Offset:X8}'.";
        }
    }
}
