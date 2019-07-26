﻿using System;
using CPUEmu.Interfaces;

namespace CPUEmu.Aarch32.Instructions.DataProcessing
{
    class DataProcessingAdcInstruction : DataProcessingInstruction
    {
        protected override bool IsLogical => false;

        public DataProcessingAdcInstruction(int position, byte condition, bool i, bool s, uint operand2, byte rn, byte rd) : 
            base(position, condition, i, s, operand2, rn, rd)
        {
        }

        protected override void ExecuteInternal(ICpuState cpuState, uint operand2Value)
        {
            var rn = Convert.ToUInt32(cpuState.GetRegister($"R{Rn}"));
            var rdNewValue = rn + operand2Value + Convert.ToInt32(cpuState.GetFlag("C"));

            cpuState.SetRegister($"R{Rd}", rdNewValue);
            if (S)
                SetFlagsArithmethic(cpuState, (uint)rdNewValue, rn, operand2Value, true, Convert.ToBoolean(cpuState.GetFlag("C")));
        }

        public override string ToString()
        {
            return $"ADC{(S ? "S" : "")}{ConditionHelper.ToString(Condition)} R{Rd}, R{Rn}, {(I ? $"#{GetOp2Value(null, true)}" : $"R{Operand2 & 0xF}")}";
        }
    }
}