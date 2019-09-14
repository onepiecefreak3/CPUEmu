using System.Collections.Generic;
using System.IO;
using System.Linq;
using CpuContract;
using CpuContract.Attributes;
using CpuContract.DependencyInjection;
using CpuContract.Executor;
using CpuContract.Memory;

namespace adapter_arm32Test
{
    [UniqueIdentifier("TestArm32")]
    public class Arm32TestAdapter : IAssemblyAdapter
    {
        private const int PayloadAddress = 0x100000;
        private const int StackAddress = 0x1000000;
        private const int StackSize = 0x100000;
        private const int MemorySize = 512 * 1024 * 1024;

        private IServiceProvider<IArchitectureParser> _architectureProvider;
        private IServiceProvider<ICpuState> _cpuStateProvider;
        private IServiceProvider<IInterruptBroker> _interruptProvider;
        private IServiceProvider<IExecutor> _executorProvider;

        public Arm32TestAdapter(IServiceProvider<IArchitectureParser> parserProvider, IServiceProvider<ICpuState> cpuStateProvider,
            IServiceProvider<IInterruptBroker> interruptProvider, IServiceProvider<IExecutor> executorProvider)
        {
            _architectureProvider = parserProvider;
            _cpuStateProvider = cpuStateProvider;
            _interruptProvider = interruptProvider;
            _executorProvider = executorProvider;
        }

        public bool Identify(Stream assembly)
        {
            var startPosition = assembly.Position;

            var magic = new byte[4];
            assembly.Read(magic, 0, 4);
            assembly.Position = startPosition;

            return magic.SequenceEqual(new byte[] { 0x54, 0x45, 0x53, 0x54 });
        }

        public IList<IInstruction> ParseAssembly(Stream assembly)
        {
            assembly.Position = 4;
            var parser = _architectureProvider.GetService("Aarch32");
            return parser.ParseAssembly(assembly, PayloadAddress);
        }

        public IExecutionEnvironment CreateExecutionEnvironment(Stream assembly)
        {
            // Create environment instances
            var cpuState = _cpuStateProvider.GetService("Aarch32");
            var memoryMap = new LittleEndianMemoryMap(MemorySize,
                new MemoryRegion(PayloadAddress, (int)assembly.Length), new MemoryRegion(StackAddress, StackSize));
            var interruptBroker = _interruptProvider.GetService("Default");

            // Load payload into memory
            assembly.Position = 4;
            var assemblyBuffer = new byte[assembly.Length - 4];
            assembly.Read(assemblyBuffer, 0, assemblyBuffer.Length);
            memoryMap.Write(assemblyBuffer, 0, assemblyBuffer.Length, PayloadAddress);

            return new TestArm32ExecutionEnvironment(cpuState, memoryMap, interruptBroker);
        }

        public IExecutor CreateExecutor()
        {
            return _executorProvider.GetService("Aarch32");
        }
    }
}
