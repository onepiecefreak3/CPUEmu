using System.IO;
using CpuContract;
using CpuContract.Memory;

namespace CPUEmu.MemoryManipulation
{
    public class FileMemoryManipulation : IMemoryManipulation
    {
        private string _filename;

        public int Offset { get; }

        public FileMemoryManipulation(int offset, string filename)
        {
            Offset = offset;
            _filename = filename;
        }

        public void Execute(IMemoryMap memoryMapMap)
        {
            if (!File.Exists(_filename))
                return;

            var file = File.OpenRead(_filename);
            var buffer = new byte[file.Length];
            file.Read(buffer, 0, buffer.Length);
            file.Close();

            memoryMapMap.Write(buffer, 0, buffer.Length, Offset);
        }

        public override string ToString()
        {
            return $"Set file '{Path.GetFileName(_filename)}' at offset '0x{Offset:X8}'.";
        }
    }
}
