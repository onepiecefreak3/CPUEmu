using CpuContract;
using CpuContract.Attributes;
using CpuContract.Memory;

namespace adapter_arm32Test
{
    public class TestArm32ExecutionEnvironment : IExecutionEnvironment
    {
        public ICpuState CpuState { get; private set; }
        public BaseMemoryMap MemoryMap { get; private set; }
        public IInterruptBroker InterruptBroker { get; private set; }

        public TestArm32ExecutionEnvironment(ICpuState cpuState, BaseMemoryMap memoryMap, IInterruptBroker interruptBroker)
        {
            CpuState = cpuState;
            MemoryMap = memoryMap;
            InterruptBroker = interruptBroker;
        }

        public void Reset()
        {
            CpuState.SetRegister("PC", MemoryMap.Payload.Address);
            CpuState.SetRegister("SP", MemoryMap.Stack.Address);

            for (int i = 0; i < 13; i++)
                CpuState.SetRegister("R" + i, 0);
            CpuState.SetRegister("LR", 0);

            CpuState.SetFlag("C",false);
            CpuState.SetFlag("N", false);
            CpuState.SetFlag("V", false);
            CpuState.SetFlag("Z", false);
        }

        public void Dispose()
        {
            MemoryMap.Dispose();
            InterruptBroker.Dispose();

            CpuState = null;
            MemoryMap = null;
            InterruptBroker = null;
        }
    }
}
