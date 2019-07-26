using System;

namespace CPUEmu.AARCH32.Factories
{
    class DataProcessingInstructionFactory
    {
        public static Instructions.DataProcessing.DataProcessingInstruction Create(int position, byte condition, uint instruction)
        {
            var operand2 = instruction & 0xFFF;
            var i = ((instruction >> 25) & 0x1) == 1;
            var opcode = (instruction >> 21) & 0xF;
            var s = ((instruction >> 20) & 0x1) == 1;
            var rn = (byte)((instruction >> 16) & 0xF);
            var rd = (byte)((instruction >> 12) & 0xF);

            switch (opcode)
            {
                case 0:
                    return new Instructions.DataProcessing.DataProcessingAndInstruction(position, condition, i, s, operand2, rn, rd);
                case 1:
                    return new Instructions.DataProcessing.DataProcessingEorInstruction(position, condition, i, s, operand2, rn, rd);
                case 2:
                    return new Instructions.DataProcessing.DataProcessingSubInstruction(position, condition, i, s, operand2, rn, rd);
                case 3:
                    return new Instructions.DataProcessing.DataProcessingRsbInstruction(position, condition, i, s, operand2, rn, rd);
                case 4:
                    return new Instructions.DataProcessing.DataProcessingAddInstruction(position, condition, i, s, operand2, rn, rd);
                case 5:
                    return new Instructions.DataProcessing.DataProcessingAdcInstruction(position, condition, i, s, operand2, rn, rd);
                case 6:
                    return new Instructions.DataProcessing.DataProcessingSbcInstruction(position, condition, i, s, operand2, rn, rd);
                case 7:
                    return new Instructions.DataProcessing.DataProcessingRscInstruction(position, condition, i, s, operand2, rn, rd);
                case 8:
                    return new Instructions.DataProcessing.DataProcessingTstInstruction(position, condition, i, s, operand2, rn, rd);
                case 9:
                    return new Instructions.DataProcessing.DataProcessingTeqInstruction(position, condition, i, s, operand2, rn, rd);
                case 10:
                    return new Instructions.DataProcessing.DataProcessingCmpInstruction(position, condition, i, s, operand2, rn, rd);
                case 11:
                    return new Instructions.DataProcessing.DataProcessingCmnInstruction(position, condition, i, s, operand2, rn, rd);
                case 12:
                    return new Instructions.DataProcessing.DataProcessingOrrInstruction(position, condition, i, s, operand2, rn, rd);
                case 13:
                    return new Instructions.DataProcessing.DataProcessingMovInstruction(position, condition, i, s, operand2, rn, rd);
                case 14:
                    return new Instructions.DataProcessing.DataProcessingBicInstruction(position, condition, i, s, operand2, rn, rd);
                case 15:
                    return new Instructions.DataProcessing.DataProcessingMvnInstruction(position, condition, i, s, operand2, rn, rd);
                default:
                    throw new InvalidOperationException($"Opcode {opcode} is unknown.");
            }
        }
    }
}
