﻿using System.Collections.Generic;
using System.IO;
using architecture_avr.Instructions;
using architecture_avr.Instructions.ArithmeticLogical;
using architecture_avr.Instructions.Branch;
using architecture_avr.Instructions.DataTransfer;
using CpuContract;
using CpuContract.Exceptions;

/* http://ww1.microchip.com/downloads/en/devicedoc/atmel-0856-avr-instruction-set-manual.pdf */
/* https://en.wikipedia.org/wiki/Atmel_AVR_instruction_set */

namespace architecture_avr
{
    class AvrInstructionParser : IExecutableInstructionParser<AvrCpuState>
    {
        public IList<IExecutableInstruction<AvrCpuState>> Instructions { get; private set; }

        public void LoadPayload(Stream file, int baseAddress)
        {
            Instructions = new List<IExecutableInstruction<AvrCpuState>>();
            var startPosition = file.Position;

            while (file.Position < file.Length)
            {
                var instructionPosition = (int)(baseAddress + file.Position - startPosition);
                var instruction = ReadUInt16(file);

                if ((instruction & 0xFC00) == 0)
                {
                    if (instruction == 0)
                        Instructions.Add(new NopInstruction(instructionPosition));
                    else if ((instruction & 0x300) == 0x100)
                        Instructions.Add(new MovwInstruction(instructionPosition, (instruction & 0xF0) >> 4, instruction & 0xF));
                    else if ((instruction & 0x300) == 0x200)
                        Instructions.Add(new MulsInstruction(instructionPosition, (instruction & 0xF0) >> 4, instruction & 0xF));
                    else if ((instruction & 0x388) == 0x300)
                        Instructions.Add(new MulsuInstruction(instructionPosition, (instruction & 0x70) >> 4, instruction & 0x7));
                    else if ((instruction & 0x388) == 0x308)
                        Instructions.Add(new FmulInstruction(instructionPosition, (instruction & 0x70) >> 4, instruction & 0x7));
                    else if ((instruction & 0x380) == 0x380)
                        Instructions.Add(new FmulsInstruction(instructionPosition, (instruction & 0x70) >> 4, instruction & 0x7, (instruction & 0x8) >> 3 == 1));
                    else
                        throw new UndefinedInstructionException(instruction, instructionPosition);
                }
                else if ((instruction & 0xE000) == 0)
                {
                    if ((instruction & 0xC00) == 0x400)
                        Instructions.Add(new CmpInstruction(instructionPosition, (instruction & 0x1F0) >> 4, ((instruction & 0x200) >> 5) | (instruction & 0xF), (instruction & 1000) == 0));
                    else if ((instruction & 0xC00) == 0x800)
                        Instructions.Add(new SubInstruction(instructionPosition, (instruction & 0x1F0) >> 4, ((instruction & 0x200) >> 5) | (instruction & 0xF), (instruction & 1000) == 0));
                    else if ((instruction & 0xC00) == 0xC00)
                        Instructions.Add(new AddInstruction(instructionPosition, (instruction & 0x1F0) >> 4, ((instruction & 0x200) >> 5) | (instruction & 0xF), (instruction & 1000) == 1));
                    else if ((instruction & 0x1C00) == 0x1000)
                        Instructions.Add(new CpseInstruction(instructionPosition, (instruction & 0x1F0) >> 4, ((instruction & 0x200) >> 5) | (instruction & 0xF)));
                    else
                        throw new UndefinedInstructionException(instruction, instructionPosition);
                }
                else if ((instruction & 0xFC00) < 0x3000)
                {
                    if ((instruction & 0xC00) == 0)
                        Instructions.Add(new AndInstruction(instructionPosition, (instruction & 0x1F0) >> 4, ((instruction & 0x200) >> 5) | (instruction & 0xF)));
                    else if ((instruction & 0xC00) == 0x400)
                        Instructions.Add(new EorInstruction(instructionPosition, (instruction & 0x1F0) >> 4, ((instruction & 0x200) >> 5) | (instruction & 0xF)));
                    else if ((instruction & 0xC00) == 0x800)
                        Instructions.Add(new OrInstruction(instructionPosition, (instruction & 0x1F0) >> 4, ((instruction & 0x200) >> 5) | (instruction & 0xF)));
                    else if ((instruction & 0xC00) == 0xC00)
                        Instructions.Add(new MovInstruction(instructionPosition, (instruction & 0x1F0) >> 4, ((instruction & 0x200) >> 5) | (instruction & 0xF)));
                    else
                        throw new UndefinedInstructionException(instruction, instructionPosition);
                }
                else if ((instruction & 0xF000) == 0x3000)
                {
                    Instructions.Add(new CpiInstruction(instructionPosition, (instruction & 0xF0) >> 4, (instruction & 0xF) | ((instruction & 0xF00) >> 4)));
                }
                else if ((instruction & 0xC000) == 0x4000)
                {
                    if ((instruction & 0x2000) == 0)
                        Instructions.Add(new SubInstruction(instructionPosition, (instruction & 0xF0) >> 4, ((instruction & 0xF00) >> 4) | (instruction & 0xF), (instruction & 1000) == 0, true));
                    else if ((instruction & 0x3000) == 0x2000)
                        Instructions.Add(new OrInstruction(instructionPosition, (instruction & 0xF0) >> 4, ((instruction & 0xF00) >> 4) | (instruction & 0xF), true));
                    else if ((instruction & 0x3000) == 0x3000)
                        Instructions.Add(new AndInstruction(instructionPosition, (instruction & 0xF0) >> 4, ((instruction & 0xF00) >> 4) | (instruction & 0xF), true));
                    else
                        throw new UndefinedInstructionException(instruction, instructionPosition);
                }
                else if ((instruction & 0xD000) == 0x8000)
                {
                    var rd = (instruction & 0x1F0) >> 4;
                    var s = (instruction & 0x200) != 0;
                    var y = (instruction & 0x8) != 0;
                    var dis = ((instruction & 0x2000) >> 8) | ((instruction & 0xC00) >> 7) | (instruction & 0x7);

                    if (s)
                        Instructions.Add(new StdInstruction(instructionPosition, rd, y, dis));
                    else
                        Instructions.Add(new LddInstruction(instructionPosition, rd, y, dis));
                }
                else if ((instruction & 0xFC00) == 0x9000)
                {
                    var rd = (instruction & 0x1F0) >> 4;
                    var s = (instruction & 0x200) != 0;
                    var y = (instruction & 0x8) != 0;
                    var q = (instruction & 0x2) != 0;

                    if ((instruction & 0xF) == 0)
                        Instructions.Add();
                    else if ((instruction & 0x7) == 1)
                        Instructions.Add();
                    else if ((instruction & 0x7) == 2)
                        Instructions.Add();
                    else if ((instruction & 0xD) == 4)
                        Instructions.Add();
                    else if ((instruction & 0xD) == 5)
                        Instructions.Add();
                    else if ((instruction & 0x20F) == 0x204)
                        Instructions.Add();
                    else if ((instruction & 0x20F) == 0x205)
                        Instructions.Add();
                    else if ((instruction & 0x20F) == 0x206)
                        Instructions.Add();
                    else if ((instruction & 0x20F) == 0x207)
                        Instructions.Add();
                    else if ((instruction & 0xF) == 0xC)
                        Instructions.Add();
                    else if ((instruction & 0xF) == 0xD)
                        Instructions.Add();
                    else if ((instruction & 0xF) == 0xE)
                        Instructions.Add();
                    else if ((instruction & 0xF) == 0xF)
                        Instructions.Add();
                }
                else if ((instruction & 0xFC08) == 0x9400)
                {

                }
                else if ((instruction & 0xFC08) == 0x9400 || (instruction & 0xFE08) == 0x9408)
                {

                }
                else if ((instruction & 0xFF0F) == 0x9508)
                {

                }
                else if ((instruction & 0xFE00) == 0x9400 && (instruction & 0x000C) >= 0x0008)
                {

                }
                else if ((instruction & 0xFE00) == 0x9600 || (instruction & 0x9800) == 0x9800)
                {

                }
                else if ((instruction & 0xE000) >= 0xA000)
                {

                }
                else
                {
                    throw new UndefinedInstructionException(instruction, instructionPosition);
                }

                /*
                 * CALL   1001010xxxxx111x xxxxxxxxxxxxxxxx
                 * JMP    1001010xxxxx110x xxxxxxxxxxxxxxxx
                 * LDS    1001000xxxxx0000 xxxxxxxxxxxxxxxx
                 * STS    1001001xxxxx0000 xxxxxxxxxxxxxxxx
                 */
            }
        }

        public void Dispose()
        {
            Instructions?.Clear();
            Instructions = null;
        }

        private ushort ReadUInt16(Stream input)
        {
            var value = new byte[2];
            input.Read(value, 0, 2);
            return (ushort)((value[0] << 8) | value[1]);
        }
    }
}