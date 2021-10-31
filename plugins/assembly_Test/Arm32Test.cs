using System.IO;
using System.Linq;
using CpuContract;
using CpuContract.Attributes;
using CpuContract.Executor;
using CpuContract.Memory;

namespace adapter_arm32Test
{
    [UniqueIdentifier("Arm32Test")]
    public class Arm32TestAssembly : IAssembly
    {
        private const int PayloadAddress_ = 0x100000;
        private const int StackAddress_ = 0x1000000;
        private const int StackSize_ = 0x100000;
        private const int MemorySize_ = 512 * 1024 * 1024;

        public string Architecture => "Aarch32";

        public bool CanIdentify => true;

        public bool Identify(Stream assembly)
        {
            var startPosition = assembly.Position;

            var magic = new byte[4];
            assembly.Read(magic, 0, 4);
            assembly.Position = startPosition;

            return magic.SequenceEqual(new byte[] { 0x41, 0x52, 0x33, 0x32 });
        }

        public void LoadPayload(Stream assembly, IInstructionParser instructionParser)
        {
            assembly.Position = 4;
            instructionParser.LoadPayload(assembly, PayloadAddress_);
        }

        public DeviceEnvironment CreateExecutionEnvironment(Stream assembly, IExecutor executor)
        {
            // Create environment instances
            var memoryMap = new LittleEndianMemoryMapMap(MemorySize_, PayloadAddress_, (int)assembly.Length, StackAddress_, StackSize_);

            // Load payload into memory
            var assemblyBuffer = new byte[assembly.Length - 4];

            assembly.Position = 4;
            assembly.Read(assemblyBuffer, 0, assemblyBuffer.Length);

            memoryMap.Write(assemblyBuffer, 0, assemblyBuffer.Length, PayloadAddress_);

            // Create execution environment
            return new DeviceEnvironment(memoryMap);
        }
    }
}
