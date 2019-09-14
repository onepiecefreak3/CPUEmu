using System.Collections.Generic;
using System.IO;
using assembly_aarch32.Exceptions;
using assembly_aarch32.Factories;
using assembly_aarch32.Instructions;
using assembly_aarch32.Instructions.Branch;
using assembly_aarch32.Models;
using CpuContract;
using CpuContract.Attributes;
using CpuContract.Logging;

namespace assembly_aarch32
{
    [UniqueIdentifier("Aarch32")]
    public class Aarch32ArchitectureParser : IArchitectureParser
    {
        private ILogger _logger;

        public Aarch32ArchitectureParser(ILogger logger)
        {
            _logger = logger;
        }

        public IList<IInstruction> ParseAssembly(Stream assembly, int baseAddress)
        {
            var result = new List<IInstruction>();
            var startPosition = assembly.Position;

            while (assembly.Position < assembly.Length)
            {
                var instructionPosition = (int)(baseAddress + assembly.Position - startPosition);
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
                        _logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'Multiply'.");
                        break;

                    case InstructionType.MultiplyLong:
                        // TODO: Implement Multiply long
                        _logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'MultiplyLong'.");
                        break;

                    case InstructionType.SingleDataSwap:
                        // TODO: Implement Data swap
                        break;

                    case InstructionType.BranchExchange:
                        // TODO: Implement branch exchange
                        _logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'BranchExchange'.");
                        break;

                    case InstructionType.HalfwordDataTransferReg:
                        // TODO: Implement halfword data transfer reg
                        _logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'HalfwordDataTransferReg'.");
                        break;

                    case InstructionType.HalfwordDataTransferImm:
                        // TODO: Implement halfword data transer imm
                        _logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'HalfwordDataTransferImm'.");
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
                        _logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'CoprocessorDataTransfer'.");
                        break;

                    case InstructionType.CoprocessorDataOperation:
                        _logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'CoprocessorDataOperation'.");
                        break;

                    case InstructionType.CoprocessorRegTransfer:
                        _logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'CoprocessorRegTransfer'.");
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

        public void Dispose()
        {
            // Nothing to dispose
        }

        private InstructionType GetInstructionType(uint instruction)
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

        private uint ReadUInt32(Stream input)
        {
            var value = new byte[4];
            input.Read(value, 0, 4);
            return (uint)(value[0] | (value[1] << 8) | (value[2] << 16) | (value[3] << 24));
        }
    }
}
