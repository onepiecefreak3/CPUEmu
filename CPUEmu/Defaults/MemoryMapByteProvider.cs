using System;
using Be.Windows.Forms;
using CPUEmu.Interfaces;

namespace CPUEmu.Defaults
{
    class MemoryMapByteProvider : IByteProvider
    {
        private readonly IMemoryMap _memoryMap;

        public MemoryMapByteProvider(IMemoryMap memoryMap)
        {
            _memoryMap = memoryMap;
        }

        public byte ReadByte(long index)
        {
            return _memoryMap.ReadByte((int)index);
        }

        public void WriteByte(long index, byte value)
        {
            _memoryMap.WriteByte((int)index, value);
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

        public long Length => _memoryMap.Length;

        public event EventHandler LengthChanged;
        public event EventHandler Changed;
    }
}
