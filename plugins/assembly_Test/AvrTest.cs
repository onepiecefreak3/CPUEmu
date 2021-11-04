using System.IO;
using System.Linq;
using CpuContract;
using CpuContract.Attributes;
using CpuContract.Executor;
using CpuContract.Memory.MemoryMap;

namespace adapter_arm32Test
{
    [UniqueIdentifier("AvrTest")]
    public class AvrTestAssembly : IAssembly
    {
        private const int PayloadAddress_ = 0x1000;
        private const int StackAddress_ = 0xF000;
        private const int StackSize_ = 0x1000;
        private const int MemorySize_ = 0x10000;

        public string Architecture => "Avr";
        public bool CanIdentify => true;

        public bool Identify(Stream assembly)
        {
            var startPosition = assembly.Position;

            var magic = new byte[4];
            assembly.Read(magic, 0, 4);
            assembly.Position = startPosition;

            return magic.SequenceEqual(new byte[] { 0x41, 0x56, 0x52, 0x00 });
        }

        public void LoadPayload(Stream assembly, IInstructionParser instructionParser)
        {
            assembly.Position = 4;
            instructionParser.LoadPayload(assembly, PayloadAddress_);
        }

        public DeviceEnvironment CreateExecutionEnvironment(Stream assembly, IExecutor executor)
        {
            // Create environment instances
            var memoryMap = new LittleEndianMemoryMap(MemorySize_, PayloadAddress_, (int)assembly.Length, StackAddress_, StackSize_);

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
