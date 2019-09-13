using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using CPUEmu.Aarch32;
using CPUEmu.Interfaces;

namespace CPUEmu.Assemblies
{
    [UniqueIdentifier("TestArm")]
    class TestArmAssemblyAdapter : IAssemblyAdapter
    {
        private ILogger _logger;

        public IList<IInstruction> Instructions { get; private set; }
        public IEnvironment Environment { get; private set; }
        public Executor Executor { get; private set; }

        public TestArmAssemblyAdapter(ILogger logger)
        {
            _logger = logger;
        }

        public bool Identify(Stream file)
        {
            var startPosition = file.Position;

            var magic = new byte[4];
            file.Read(magic, 0, 4);
            file.Position = startPosition;

            return magic.SequenceEqual(new byte[] { 0x54, 0x45, 0x53, 0x54 });
        }

        public void Load(Stream file)
        {
            var startPosition = file.Position;
            file.Position = startPosition + 4;

            Instructions = Aarch32ArchitectureParser.Parse(file, _logger);

            var env = new Aarch32Environment(500 * 1024 * 1024, 0x100000, 0x1000000, 0x100000);

            Environment = env;
            Executor = new Aarch32Executor(Instructions, Environment);

            file.Position = startPosition;
        }

        public void InitializeMemoryMap(Stream file)
        {
            Environment.MemoryMap.Clear();

            // Copy assembly payload into memory
            var payloadLength = file.Length;
            file.Position = 0;
            var payload = new byte[payloadLength];
            file.Read(payload, 0, (int)payloadLength);
            Environment.MemoryMap.Write(payload, 0, payload.Length, Environment.PayloadAddress);
        }

        public void Dispose()
        {
            foreach (var inst in Instructions)
                inst.Dispose();
            Environment.Dispose();
            Executor.Dispose();

            Instructions = null;
            Environment = null;
            Executor = null;
        }
    }
}
