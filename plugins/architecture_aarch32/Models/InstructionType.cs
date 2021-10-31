namespace assembly_aarch32.Models
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
}
