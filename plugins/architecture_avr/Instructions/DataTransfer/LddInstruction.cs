using CpuContract;

namespace architecture_avr.Instructions.DataTransfer
{
    class LddInstruction : BaseInstruction
    {
        private int _rd;
        private bool _isY;
        private int _dis;

        public LddInstruction(int position, int rd, bool isY, int dis) : base(position, 2)
        {
            _rd = rd;
            _isY = isY;
            _dis = dis;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var v1 = _isY ? cpuState.RegY : cpuState.RegZ;
            var res = v1 + _dis;

            cpuState.Registers[_rd] = env.MemoryMap.ReadByte(res);
        }

        public override string ToString()
        {
            return $"LDD R{_rd}, {(_isY ? "Y" : "Z")}+{_dis}";
        }
    }
}
