using System.Collections.Generic;
using System.IO;
using assembly_aarch32.Factories;
using assembly_aarch32.Instructions;
using assembly_aarch32.Instructions.Branch;
using assembly_aarch32.Models;
using CpuContract;
using CpuContract.Exceptions;
using Serilog;

/* https://iitd-plos.github.io/col718/ref/arm-instructionset.pdf */

namespace assembly_aarch32
{
    public class Aarch32InstructionParser : IExecutableInstructionParser<Aarch32CpuState>
    {
        private readonly ILogger _logger;

        public IList<IExecutableInstruction<Aarch32CpuState>> Instructions { get; private set; }

        public Aarch32InstructionParser(ILogger logger)
        {
            _logger = logger;
        }

        public void LoadPayload(Stream assembly, int baseAddress)
        {
            Instructions = new List<IExecutableInstruction<Aarch32CpuState>>();
            var startPosition = assembly.Position;

            while (assembly.Position < assembly.Length)
            {
                var instructionPosition = (int)(baseAddress + assembly.Position - startPosition);
                var instruction = ReadUInt32(assembly);
                var condition = (byte)(instruction >> 28);

                switch (GetInstructionType(instruction))
                {
                    case InstructionType.DataProcessing:
                        Instructions.Add(DataProcessingInstructionFactory.CreateInstruction(instructionPosition, condition,
                            instruction));
                        break;

                    case InstructionType.Multiply:
                        // TODO: Implement multiply
                        _logger?.Fatal("Unimplemented Instructiontype 'Multiply'.");
                        break;

                    case InstructionType.MultiplyLong:
                        // TODO: Implement Multiply long
                        _logger?.Fatal("Unimplemented Instructiontype 'MultiplyLong'.");
                        break;

                    case InstructionType.SingleDataSwap:
                        // TODO: Implement Data swap
                        _logger?.Fatal("Unimplemented Instructiontype 'SingleDataSwap'.");
                        break;

                    case InstructionType.BranchExchange:
                        Instructions.Add(BranchAndExchangeInstruction.Parse(instructionPosition, condition, instruction));
                        break;

                    case InstructionType.HalfwordDataTransferReg:
                        // TODO: Implement halfword data transfer reg
                        _logger?.Fatal("Unimplemented Instructiontype 'HalfwordDataTransferReg'.");
                        break;

                    case InstructionType.HalfwordDataTransferImm:
                        // TODO: Implement halfword data transer imm
                        _logger?.Fatal("Unimplemented Instructiontype 'HalfwordDataTransferImm'.");
                        break;

                    case InstructionType.SingleDataTransfer:
                        Instructions.Add(SingleDataTransferInstruction.Parse(instructionPosition, condition, instruction));
                        break;

                    case InstructionType.BlockDataTransfer:
                        Instructions.Add(BlockDataTransferInstruction.Parse(instructionPosition, condition, instruction));
                        break;

                    case InstructionType.Branch:
                        Instructions.Add(BranchInstruction.Parse(instructionPosition, condition, instruction));
                        break;

                    case InstructionType.CoprocessorDataTransfer:
                        _logger?.Fatal("Unimplemented Instructiontype 'CoprocessorDataTransfer'.");
                        break;

                    case InstructionType.CoprocessorDataOperation:
                        _logger?.Fatal("Unimplemented Instructiontype 'CoprocessorDataOperation'.");
                        break;

                    case InstructionType.CoprocessorRegTransfer:
                        _logger?.Fatal("Unimplemented Instructiontype 'CoprocessorRegTransfer'.");
                        break;

                    case InstructionType.SoftwareInterrupt:
                        Instructions.Add(SvcInstruction.Parse(instructionPosition, condition, instruction));
                        break;

                    default:
                        throw new UndefinedInstructionException(instruction, instructionPosition);
                }
            }
        }

        public void Dispose()
        {
            Instructions?.Clear();
            Instructions = null;
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
