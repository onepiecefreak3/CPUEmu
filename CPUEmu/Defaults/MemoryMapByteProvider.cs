using System;
using Be.Windows.Forms;
using CpuContract;
using CpuContract.Memory;

namespace CPUEmu.Defaults
{
    class MemoryMapByteProvider : IByteProvider
    {
        private readonly IMemoryMap _memoryMapMap;

        public MemoryMapByteProvider(IMemoryMap memoryMapMap)
        {
            _memoryMapMap = memoryMapMap;
        }

        public byte ReadByte(long index)
        {
            return _memoryMapMap.ReadByte((int)index);
        }

        public void WriteByte(long index, byte value)
        {
            _memoryMapMap.WriteByte((int)index, value);
        }

        public void InsertBytes(long index, byte[] bs)
        {
            throw new NotSupportedException();
        }

        public void DeleteBytes(long index, long length)
        {
            throw new NotSupportedException();
        }

        public bool HasChanges()
        {
            return false;
        }

        public void ApplyChanges()
        {
        }

        public bool SupportsWriteByte()
        {
            return true;
        }

        public bool SupportsInsertBytes()
        {
            return false;
        }

        public bool SupportsDeleteBytes()
        {
            return false;
        }

        public long Length => _memoryMapMap.Length;

        public event EventHandler LengthChanged;
        public event EventHandler Changed;
    }
}
