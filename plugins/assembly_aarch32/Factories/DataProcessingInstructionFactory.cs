using System;
using assembly_aarch32.Instructions.DataProcessing;
using CpuContract;

namespace assembly_aarch32.Factories
{
    class DataProcessingInstructionFactory
    {
        public static IInstruction Create(int position, byte condition, uint instruction)
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
                    return new DataProcessingAndInstruction(position, condition, i, s, operand2, rn, rd);
                case 1:
                    return new DataProcessingEorInstruction(position, condition, i, s, operand2, rn, rd);
                case 2:
                    return new DataProcessingSubInstruction(position, condition, i, s, operand2, rn, rd);
                case 3:
                    return new DataProcessingRsbInstruction(position, condition, i, s, operand2, rn, rd);
                case 4:
                    return new DataProcessingAddInstruction(position, condition, i, s, operand2, rn, rd);
                case 5:
                    return new DataProcessingAdcInstruction(position, condition, i, s, operand2, rn, rd);
                case 6:
                    return new DataProcessingSbcInstruction(position, condition, i, s, operand2, rn, rd);
                case 7:
                    return new DataProcessingRscInstruction(position, condition, i, s, operand2, rn, rd);
                case 8:
                    return new DataProcessingTstInstruction(position, condition, i, s, operand2, rn, rd);
                case 9:
                    return new DataProcessingTeqInstruction(position, condition, i, s, operand2, rn, rd);
                case 10:
                    return new DataProcessingCmpInstruction(position, condition, i, s, operand2, rn, rd);
                case 11:
                    return new DataProcessingCmnInstruction(position, condition, i, s, operand2, rn, rd);
                case 12:
                    return new DataProcessingOrrInstruction(position, condition, i, s, operand2, rn, rd);
                case 13:
                    return new DataProcessingMovInstruction(position, condition, i, s, operand2, rn, rd);
                case 14:
                    return new DataProcessingBicInstruction(position, condition, i, s, operand2, rn, rd);
                case 15:
                    return new DataProcessingMvnInstruction(position, condition, i, s, operand2, rn, rd);
                default:
                    throw new InvalidOperationException($"Opcode {opcode} is unknown.");
            }
        }
    }
}
