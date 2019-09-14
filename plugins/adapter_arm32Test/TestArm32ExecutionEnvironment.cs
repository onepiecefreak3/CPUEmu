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
            CpuState.SetRegister("PC",MemoryMap.Payload.Address);
            CpuState.SetRegister("SP", MemoryMap.Stack.Address);
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
