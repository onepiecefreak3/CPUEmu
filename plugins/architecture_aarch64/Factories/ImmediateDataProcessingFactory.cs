using System;
using System.Collections.Generic;
using System.Text;
using assembly_aarch64.Exceptions;
using assembly_aarch64.Instructions.DataProcessing;
using CpuContract;

namespace assembly_aarch64.Factories
{
    class ImmediateDataProcessingFactory
    {
        public static IInstruction Create(uint instruction,int position)
        {
            var baseGroup = instruction & 0x03000000;
            var group = instruction & 0x03800000;

            if (baseGroup == 0)
                return PcRelativeDataProcessing.Parse(instruction,position);
            if (baseGroup == 0x01000000)
                return ImmediateAddSubDataProcessing.Parse(instruction, position);

            switch (group)
            {
                case 0x02000000:
                    return ImmediateLogicalDataProcessing.Parse(instruction, position);

                case 0x02800000:
                    return ImmediateMoveWideDataProcessing.Parse(instruction, position);

                case 0x03000000:
                    return BitfieldDataProcessing.Parse(instruction, position);

                case 0x03800000:
                    return ExtractDataProcessing.Parse(instruction, position);

                default:
                    throw new UndefinedInstructionException(instruction, position);
            }
        }
    }
}
