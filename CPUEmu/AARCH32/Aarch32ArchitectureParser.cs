using System.Collections.Generic;
using System.IO;
using CPUEmu.Aarch32.Exceptions;
using CPUEmu.Interfaces;
using CPUEmu.Aarch32.Instructions;
using CPUEmu.Aarch32.Factories;
using CPUEmu.Aarch32.Instructions.Branch;

namespace CPUEmu.Aarch32
{
    class Aarch32ArchitectureParser
    {
        public static IList<IInstruction> Parse(Stream assembly, ILogger logger = null)
        {
            var result = new List<IInstruction>();
            var startPosition = assembly.Position;

            while (assembly.Position < assembly.Length)
            {
                var instructionPosition = (int)(assembly.Position - startPosition);
                var instruction = ReadUInt32(assembly);
                var condition = (byte)(instruction >> 28);

                switch (GetInstructionType(instruction))
                {
                    case InstructionType.DataProcessing:
                        result.Add(DataProcessingInstructionFactory.Create(instructionPosition, condition,
                            instruction));
                        break;

                    case InstructionType.Multiply:
                        // TODO: Implement multiply
                        logger?.Log(LogLevel.Fatal,"Unimplemented Instructiontype 'Multiply'.");
                        break;

                    case InstructionType.MultiplyLong:
                        // TODO: Implement Multiply long
                        logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'MultiplyLong'.");
                        break;

                    case InstructionType.SingleDataSwap:
                        // TODO: Implement Data swap
                        break;

                    case InstructionType.BranchExchange:
                        // TODO: Implement branch exchange
                        logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'BranchExchange'.");
                        break;

                    case InstructionType.HalfwordDataTransferReg:
                        // TODO: Implement halfword data transfer reg
                        logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'HalfwordDataTransferReg'.");
                        break;

                    case InstructionType.HalfwordDataTransferImm:
                        // TODO: Implement halfword data transer imm
                        logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'HalfwordDataTransferImm'.");
                        break;

                    case InstructionType.SingleDataTransfer:
                        result.Add(SingleDataTransferInstruction.Parse(instructionPosition, condition, instruction));
                        break;

                    case InstructionType.BlockDataTransfer:
                        result.Add(BlockDataTransferInstruction.Parse(instructionPosition, condition, instruction));
                        break;

                    case InstructionType.Branch:
                        result.Add(BranchInstruction.Parse(instructionPosition, condition, instruction));
                        break;

                    case InstructionType.CoprocessorDataTransfer:
                        logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'CoprocessorDataTransfer'.");
                        break;

                    case InstructionType.CoprocessorDataOperation:
                        logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'CoprocessorDataOperation'.");
                        break;

                    case InstructionType.CoprocessorRegTransfer:
                        logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'CoprocessorRegTransfer'.");
                        break;

                    case InstructionType.SoftwareInterrupt:
                        result.Add(SvcInstruction.Parse(instructionPosition, condition, instruction));
                        break;

                    case InstructionType.Undefined:
                    default:
                        throw new UndefinedInstructionException(instruction, instructionPosition);
                }
            }

            return result;
        }

        private static InstructionType GetInstructionType(uint instruction)
        {
            var check = instruction & 0x0C000000;
            if (check == 0x0)
            {
                if ((instruction & 0x03FFFFF0) == 0x012FFF10)
                    return InstructionType.BranchExchange;
                if ((instruction & 0x03B00FF0) == 0x01000090)
                    return InstructionType.SingleDataSwap;
                if ((instruction & 0x02400F90) == 0x00000090)
                    return InstructionType.HalfwordDataTransferReg;
                if ((instruction & 0x03C00090) == 0x00000090)
                    return InstructionType.Multiply;
                if ((instruction & 0x03800090) == 0x00800090)
                    return InstructionType.MultiplyLong;
                if ((instruction & 0x02400090) == 0x00400090)
                    return InstructionType.HalfwordDataTransferImm;

                return InstructionType.DataProcessing;
            }
            if (check == 0x04000000)
            {
                if ((instruction & 0x02000010) == 0x02000010)
                    return InstructionType.Undefined;

                return InstructionType.SingleDataTransfer;
            }
            if (check == 0x08000000)
            {
                if ((instruction & 0x02000000) == 0x0)
                    return InstructionType.BlockDataTransfer;

                return InstructionType.Branch;
            }
            if (check == 0x0C000000)
            {
                if ((instruction & 0x02000000) == 0x0)
                    return InstructionType.CoprocessorDataTransfer;
                if ((instruction & 0x03000010) == 0x02000000)
                    return InstructionType.CoprocessorDataOperation;
                if ((instruction & 0x03000010) == 0x02000010)
                    return InstructionType.CoprocessorRegTransfer;

                return InstructionType.SoftwareInterrupt;
            }

            return InstructionType.Undefined;
        }

        private static uint ReadUInt32(Stream input)
        {
            var value = new byte[4];
            input.Read(value, 0, 4);
            return (uint)(value[0] | (value[1] << 8) | (value[2] << 16) | (value[3] << 24));
        }
    }
}
