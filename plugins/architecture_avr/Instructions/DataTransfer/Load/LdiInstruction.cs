using CpuContract;

namespace architecture_avr.Instructions.DataTransfer.Load
{
    // HINT: Acts as SER instruction if immediate is 0xFF
    class LdiInstruction : BaseInstruction
    {
        private int _rd;
        private byte _imm;

        public LdiInstruction(int position, int rd, byte imm) : base(position, 2)
        {
            _rd = rd;
            _imm = imm;
        }

        public override void Execute(AvrCpuState cpuState, DeviceEnvironment env)
        {
            cpuState.Registers[_rd] = _imm;
        }

        public override string ToString()
        {
            return _imm == 0xFF ? $"SER R{_rd}" : $"LDI R{_rd}, {_imm}";
        }
    }
}
