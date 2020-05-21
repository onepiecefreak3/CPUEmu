using System;
using System.Collections.Generic;
using System.Text;

namespace assembly_aarch64
{
    enum InstructionType
    {
        Undefined,
        ImmediateDataProcessing,
        RegisterDataProcessing,
        SimdFloatDataProcessing1,
        SimdFloatDataProcessing2,
        BranchAndSvc,
        LoadAndStore
    }
}
