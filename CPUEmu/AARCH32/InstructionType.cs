using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu
{
    enum InstructionType
    {
        DataProcessing,
        Multiply,
        MultiplyLong,
        SingleDataSwap,
        BranchExchange,
        HalfwordDataTransferReg,
        HalfwordDataTransferImm,
        SingleDataTransfer,
        Undefined,
        BlockDataTransfer,
        Branch,
        CoprocessorDataTransfer,
        CoprocessorDataOperation,
        CoprocessorRegTransfer,
        SoftwareInterrupt
    }

    private void ExecuteInstructionType(uint instruction)
    {
        switch (GetInstructionType(instruction))
        {
            case InstructionType.DataProcessing:
                DataProcessing(instruction);
                Log?.Invoke(this, "Data Processing");
                break;

            case InstructionType.Multiply:
                break;

            case InstructionType.MultiplyLong:
                break;

            case InstructionType.SingleDataSwap:
                break;

            case InstructionType.BranchExchange:
                BranchExchange(instruction);
                Log?.Invoke(this, "Branch and Exchange");
                break;

            case InstructionType.HalfwordDataTransferReg:
                break;

            case InstructionType.HalfwordDataTransferImm:
                break;

            case InstructionType.SingleDataTransfer:
                SingleDataTransfer(instruction);
                Log?.Invoke(this, "Single Data Transfer");
                break;

            case InstructionType.BlockDataTransfer:
                BlockDataTransfer(instruction);
                Log?.Invoke(this, "Block Data Transfer");
                break;

            case InstructionType.Branch:
                Branch(instruction);
                Log?.Invoke(this, "Branch");
                break;

            case InstructionType.CoprocessorDataTransfer:
                break;

            case InstructionType.CoprocessorDataOperation:
                break;

            case InstructionType.CoprocessorRegTransfer:
                break;

            case InstructionType.SoftwareInterrupt:
                Log?.Invoke(this, $"Software interrupt at PC 0x{_pc - 8:X8}");
                break;

            case InstructionType.Undefined:
            default:
                Log?.Invoke(this, $"Instruction 0x{instruction:X8} undefined at PC 0x{_pc - 8:X8}");
                break;
        }
    }

    //private string DisassembleInstructionType(uint instruction)
    //{
    //    switch (GetInstructionType(instruction))
    //    {
    //        case InstructionType.DataProcessing:
    //            return DisassembleDataProcessing(instruction);

    //        case InstructionType.Multiply:
    //            break;

    //        case InstructionType.MultiplyLong:
    //            break;

    //        case InstructionType.SingleDataSwap:
    //            break;

    //        case InstructionType.BranchExchange:
    //            return DisassembleBranchExchange(instruction);

    //        case InstructionType.HalfwordDataTransferReg:
    //            break;

    //        case InstructionType.HalfwordDataTransferImm:
    //            break;

    //        case InstructionType.SingleDataTransfer:
    //            return DisassembleSingleDataTransfer(instruction);

    //        case InstructionType.BlockDataTransfer:
    //            return DisassembleBlockDataTransfer(instruction);

    //        case InstructionType.Branch:
    //            return DisassembleBranch(instruction);

    //        case InstructionType.CoprocessorDataTransfer:
    //            break;

    //        case InstructionType.CoprocessorDataOperation:
    //            break;

    //        case InstructionType.CoprocessorRegTransfer:
    //            break;

    //        case InstructionType.SoftwareInterrupt:
    //            return $"SWI 0x{instruction & 0xFFFFFF:X8}";

    //        case InstructionType.Undefined:
    //        default:
    //            return "NOP";
    //    }

    //    return "";
    //}

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
