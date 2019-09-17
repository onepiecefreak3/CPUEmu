using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using assembly_aarch32.Support;
using CpuContract;

namespace assembly_aarch32.Instructions.Branch
{
    class BranchAndExchangeInstruction : IInstruction
    {
        private byte _condition;
        private int _rn;

        public bool IsThumbMode => (_rn & 0x1) == 1;

        public int Position { get; }

        private BranchAndExchangeInstruction(int position, byte condition, int rn)
        {
            Position = position;

            _condition = condition;
            _rn = rn;
        }

        public static IInstruction Parse(int position, byte condition, uint instruction)
        {
            var rn = (int)(instruction & 0xF);

            return new BranchAndExchangeInstruction(position, condition, rn);
        }

        public void Execute(IExecutionEnvironment env)
        {
            switch (env.CpuState)
            {
                case Aarch32CpuState armCpuState:
                    if (!ConditionHelper.CanExecute(armCpuState, _condition))
                        return;

                    armCpuState.PC = armCpuState.Registers[_rn];
                    break;
                default:
                    throw new InvalidOperationException("Unknown cpu state.");
            }
        }

        public override string ToString()
        {
            var result = "BX";
            result += ConditionHelper.ToString(_condition);
            result += $" R{_rn}";
            return result;
        }

        public void Dispose()
        {
            // Nothing to dipose
        }
    }
}
