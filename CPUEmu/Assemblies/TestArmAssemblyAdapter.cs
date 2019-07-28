using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using CPUEmu.Aarch32;
using CPUEmu.Interfaces;

namespace CPUEmu.Assemblies
{
    [Export(typeof(IAssemblyAdapter))]
    class TestArmAssemblyAdapter : IAssemblyAdapter
    {
        public IList<IInstruction> Instructions { get; private set; }
        public IEnvironment Environment { get; private set; }
        public Executor Executor { get; private set; }

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

            Instructions = Aarch32ArchitectureParser.Parse(file);

            var env = new Aarch32Environment(500 * 1024 * 1024, 0x100000, 0x1000000, 0x100000);

            // Copy assembly payload into memory
            var payloadLength = file.Position - startPosition;
            file.Position = startPosition;
            var payload = new byte[payloadLength];
            file.Read(payload, 0, (int)payloadLength);
            env.MemoryMap.Write(payload, 0, payload.Length, env.PayloadAddress);

            Environment = env;
            Executor = new Aarch32Executor(Instructions, Environment);

            file.Position = startPosition;
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
