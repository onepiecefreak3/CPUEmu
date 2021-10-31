using System;
using System.Collections.Generic;
using System.IO;
using assembly_aarch64.Exceptions;
using assembly_aarch64.Factories;
using CpuContract;
using CpuContract.Attributes;
using CpuContract.Logging;

namespace assembly_aarch64
{
    [UniqueIdentifier("Aarch64")]
    public class Aarch64InstructionParser : IInstructionParser
    {
        private ILogger _logger;

        public Aarch64InstructionParser(ILogger logger)
        {
            _logger = logger;
        }

        public IList<IInstruction> LoadPayload(Stream assembly, int baseAddress)
        {
            var result = new List<IInstruction>();
            var startPosition = assembly.Position;

            while (assembly.Position < assembly.Length)
            {
                var instructionPosition = (int)(baseAddress + assembly.Position - startPosition);
                var instruction = ReadUInt32(assembly);

                switch (GetInstructionType(instruction))
                {
                    case InstructionType.ImmediateDataProcessing:
                        result.Add(ImmediateDataProcessingFactory.Create(instruction, instructionPosition));
                        break;

                    case InstructionType.RegisterDataProcessing:
                        _logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'RegisterDataProcessing'.");
                        break;

                    case InstructionType.LoadAndStore:
                        _logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'LoadAndStore'.");
                        break;

                    case InstructionType.BranchAndSvc:
                        _logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'BranchAndSvc'.");
                        break;

                    case InstructionType.SimdFloatDataProcessing1:
                        _logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'SimdFloatDataProcessing1'.");
                        break;

                    case InstructionType.SimdFloatDataProcessing2:
                        _logger?.Log(LogLevel.Fatal, "Unimplemented Instructiontype 'SimdFloatDataProcessing2'.");
                        break;

                    case InstructionType.Undefined:
                    default:
                        throw new UndefinedInstructionException(instruction, instructionPosition);
                }
            }

            return result;
        }

        private InstructionType GetInstructionType(uint instruction)
        {
            if ((instruction & 0x18000000) == 0)
                return InstructionType.Undefined;

            if ((instruction & 0x08000000) == 0)
                return (instruction & 0x04000000) == 0 ?
                    InstructionType.ImmediateDataProcessing :
                    InstructionType.BranchAndSvc;

            if ((instruction & 0x02000000) == 0)
                return InstructionType.LoadAndStore;

            if ((instruction & 0x04000000) == 0)
                return InstructionType.RegisterDataProcessing;

            return (instruction & 0x10000000) == 0 ?
                InstructionType.SimdFloatDataProcessing1 :
                InstructionType.SimdFloatDataProcessing2;
        }

        private uint ReadUInt32(Stream input)
        {
            var value = new byte[4];
            input.Read(value, 0, 4);
            return (uint)(value[0] | (value[1] << 8) | (value[2] << 16) | (value[3] << 24));
        }

        public void Dispose()
        {
            _logger = null;
        }
    }
}
