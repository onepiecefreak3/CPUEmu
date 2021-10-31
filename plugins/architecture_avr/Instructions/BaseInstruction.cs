using CpuContract;

namespace architecture_avr.Instructions
{
    abstract class BaseInstruction : IExecutableInstruction<AvrCpuState>
    {
        public int Position { get; }
        public uint Length { get; }

        public BaseInstruction(int position, uint length)
        {
            Position = position;
            Length = length;
        }

        public abstract void Execute(AvrCpuState cpuState, DeviceEnvironment env);

        public virtual void Dispose()
        {
        }
    }
}
