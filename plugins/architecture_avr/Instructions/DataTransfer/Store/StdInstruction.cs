using CpuContract;

namespace architecture_avr.Instructions.DataTransfer.Store
{
    class StdInstruction : BaseInstruction
    {
        private int _rd;
        private bool _isY;
        private int _dis;

        public StdInstruction(int position, int rd, bool isY, int dis) : base(position, 2)
        {
            _rd = rd;
            _isY = isY;
            _dis = dis;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            var v1 = _isY ? cpuState.RegY : cpuState.RegZ;
            var res = v1 + _dis;

            env.MemoryMap.WriteByte(res, cpuState.Registers[_rd]);
        }

        public override string ToString()
        {
            return $"STD {(_isY ? "Y" : "Z")}+{_dis}, R{_rd}";
        }
    }
}
