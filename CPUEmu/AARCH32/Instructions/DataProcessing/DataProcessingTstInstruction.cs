﻿using System;
using CPUEmu.Interfaces;

namespace CPUEmu.Aarch32.Instructions.DataProcessing
{
    class DataProcessingTstInstruction : DataProcessingInstruction
    {
        protected override bool IsLogical => true;

        public DataProcessingTstInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd) : 
            base(position, condition, i, s, operand2, rn, rd)
        {
        }

        protected override void ExecuteInternal(ICpuState cpuState, uint operand2Value)
        {
            var rn = Convert.ToUInt32(cpuState.GetRegister($"R{Rn}"));
            var res = rn & operand2Value;
            SetFlagsLogical(cpuState, res);
        }

        public override string ToString()
        {
            return $"TST{ConditionHelper.ToString(Condition)} R{Rn}, {(I ? $"#{GetOp2Value(null,true)}" : $"R{Operand2 & 0xF}")}";
        }
    }
}