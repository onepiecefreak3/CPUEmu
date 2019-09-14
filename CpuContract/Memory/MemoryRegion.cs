namespace CpuContract.Memory
{
    public struct MemoryRegion
    {
        public int Address { get; }

        public int Length { get; }

        public MemoryRegion(int address, int length)
        {
            Address = address;
            Length = length;
        }
    }
}
