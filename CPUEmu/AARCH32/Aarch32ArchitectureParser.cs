using System.Collections.Generic;
using System.IO;
using CPUEmu.Interfaces;

namespace CPUEmu.AARCH32
{
    class Aarch32ArchitectureParser : IArchitectureParser
    {
        public IList<IInstruction> Parse(Stream assembly, IInterruptBroker interruptBroker = null)
        {
            var result = new List<IInstruction>();

            using (var br = new BinaryReader(assembly))
            {
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    var instructionPosition = (int)br.BaseStream.Position;
                    var instruction = br.ReadUInt32();
                    var condition = (byte)(instruction >> 28);

                    // TODO: Make endian invariant
                    switch (GetInstructionType(instruction))
                    {
                        case InstructionType.DataProcessing:
                            result.Add(Factories.DataProcessingInstructionFactory.Create(instructionPosition, condition,
                                instruction));
                            break;

                        case InstructionType.Multiply:
                            // TODO: Implement multiply
                            break;

                        case InstructionType.MultiplyLong:
                            // TODO: Implement Multiply long
                            break;

                        case InstructionType.SingleDataSwap:
                            // TODO: Implement Data swap
                            break;

                        case InstructionType.BranchExchange:
                            BranchExchange(instruction);
                            break;

                        case InstructionType.HalfwordDataTransferReg:
                            // TODO: Implement halfword data transfer reg
                            break;

                        case InstructionType.HalfwordDataTransferImm:
                            // TODO: Implement halfword data transer imm
                            break;

                        case InstructionType.SingleDataTransfer:
                            SingleDataTransfer(instruction);
                            break;

                        case InstructionType.BlockDataTransfer:
                            BlockDataTransfer(instruction);
                            break;

                        case InstructionType.Branch:
                            result.Add(Instructions.DataProcessing.BranchInstruction.Parse(instructionPosition, condition, instruction));
                            break;

                        case InstructionType.CoprocessorDataTransfer:
                            break;

                        case InstructionType.CoprocessorDataOperation:
                            break;

                        case InstructionType.CoprocessorRegTransfer:
                            break;

                        case InstructionType.SoftwareInterrupt:
                            // TODO: Get interrupt value
                            result.Add(new Instructions.DataProcessing.SvcInstruction(instructionPosition, 0, interruptBroker));
                            break;

                        case InstructionType.Undefined:
                        default:
                            //Log?.Invoke(this, $"Instruction 0x{instruction:X8} undefined at PC 0x{_pc - 8:X8}");
                            break;
                    }
                }
            }

            return result;
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
    }
}
