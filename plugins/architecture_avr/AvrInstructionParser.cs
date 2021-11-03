using System.Collections.Generic;
using System.IO;
using architecture_avr.Instructions;
using architecture_avr.Instructions.Arithmetic;
using architecture_avr.Instructions.Bitwise;
using architecture_avr.Instructions.Branch;
using architecture_avr.Instructions.Call;
using architecture_avr.Instructions.DataTransfer;
using architecture_avr.Instructions.DataTransfer.Load;
using architecture_avr.Instructions.DataTransfer.Store;
using architecture_avr.Models;
using CpuContract;
using CpuContract.Exceptions;
using Serilog;

/* http://ww1.microchip.com/downloads/en/devicedoc/atmel-0856-avr-instruction-set-manual.pdf */
/* https://en.wikipedia.org/wiki/Atmel_AVR_instruction_set */

namespace architecture_avr
{
    // TODO: Remove bit pattern checks with > and >=, since they lead to false positives
    class AvrInstructionParser : IExecutableInstructionParser<AvrCpuState>
    {
        private ILogger _logger;

        public IList<IExecutableInstruction<AvrCpuState>> Instructions { get; private set; }

        public AvrInstructionParser(ILogger logger = null)
        {
            _logger = logger;
        }

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
                    //var q = (instruction & 0x2) != 0;

                    if ((instruction & 0xF) == 0)
                        if (s)
                            Instructions.Add(new StsInstruction(instructionPosition, rd, ReadUInt16(file)));
                        else
                            Instructions.Add(new LdsInstruction(instructionPosition, rd, ReadUInt16(file)));
                    else if ((instruction & 0x7) == 1)
                        if (s)
                            Instructions.Add(new StInstruction(instructionPosition, rd, y ? Pointer.Y : Pointer.Z, PointerModification.PostIncrement));
                        else
                            Instructions.Add(new LdInstruction(instructionPosition, rd, y ? Pointer.Y : Pointer.Z, PointerModification.PostIncrement));
                    else if ((instruction & 0x7) == 2)
                        if (s)
                            Instructions.Add(new StInstruction(instructionPosition, rd, y ? Pointer.Y : Pointer.Z, PointerModification.PreDecrement));
                        else
                            Instructions.Add(new LdInstruction(instructionPosition, rd, y ? Pointer.Y : Pointer.Z, PointerModification.PreDecrement));
                    else if ((instruction & 0xD) == 4)
                        _logger?.Fatal("Unimplemented Instructiontype 'ELPM Rd,Z'.");
                    else if ((instruction & 0xD) == 5)
                        _logger?.Fatal("Unimplemented Instructiontype 'ELPM Rd,Z+'.");
                    else if ((instruction & 0x20F) == 0x204)
                        Instructions.Add(new XchInstruction(instructionPosition, rd));
                    else if ((instruction & 0x20F) == 0x205)
                        Instructions.Add(new LasInstruction(instructionPosition, rd));
                    else if ((instruction & 0x20F) == 0x206)
                        Instructions.Add(new LacInstruction(instructionPosition, rd));
                    else if ((instruction & 0x20F) == 0x207)
                        Instructions.Add(new LatInstruction(instructionPosition, rd));
                    else if ((instruction & 0xF) == 0xC)
                        if (s)
                            Instructions.Add(new StInstruction(instructionPosition, rd, Pointer.X, PointerModification.None));
                        else
                            Instructions.Add(new LdInstruction(instructionPosition, rd, Pointer.X, PointerModification.None));
                    else if ((instruction & 0xF) == 0xD)
                        if (s)
                            Instructions.Add(new StInstruction(instructionPosition, rd, Pointer.X, PointerModification.PostIncrement));
                        else
                            Instructions.Add(new LdInstruction(instructionPosition, rd, Pointer.X, PointerModification.PostIncrement));
                    else if ((instruction & 0xF) == 0xE)
                        if (s)
                            Instructions.Add(new StInstruction(instructionPosition, rd, Pointer.X, PointerModification.PreDecrement));
                        else
                            Instructions.Add(new LdInstruction(instructionPosition, rd, Pointer.X, PointerModification.PreDecrement));
                    else if ((instruction & 0xF) == 0xF)
                        if (s)
                            Instructions.Add(new PushInstruction(instructionPosition, rd));
                        else
                            Instructions.Add(new PopInstruction(instructionPosition, rd));
                }
                else if ((instruction & 0xFE08) == 0x9400)
                {
                    var rd = (instruction & 0x1F0) >> 4;

                    if ((instruction & 0xF) == 0)
                        Instructions.Add(new ComInstruction(instructionPosition, rd));
                    else if ((instruction & 0xF) == 1)
                        Instructions.Add(new NegInstruction(instructionPosition, rd));
                    else if ((instruction & 0xF) == 2)
                        Instructions.Add(new SwapInstruction(instructionPosition, rd));
                    else if ((instruction & 0xF) == 3)
                        Instructions.Add(new IncInstruction(instructionPosition, rd));
                    else if ((instruction & 0xF) == 5)
                        Instructions.Add(new AsrInstruction(instructionPosition, rd));
                    else if ((instruction & 0xF) == 6)
                        Instructions.Add(new LsrInstruction(instructionPosition, rd));
                    else if ((instruction & 0xF) == 7)
                        Instructions.Add(new RorInstruction(instructionPosition, rd));
                }
                else if ((instruction & 0xFF0F) == 0x9408)
                {
                    if ((instruction & 0x80) != 0)
                        Instructions.Add(new ClInstruction(instructionPosition, (Flag)((instruction & 0x70) >> 4)));
                    else
                        Instructions.Add(new SeInstruction(instructionPosition, (Flag)((instruction & 0x70) >> 4)));
                }
                else if ((instruction & 0xFF0F) == 0x9508)
                {
                    if ((instruction & 0xF0) == 0)
                        Instructions.Add(new RetInstruction(instructionPosition));
                    else if ((instruction & 0xF0) == 0x10)
                        Instructions.Add(new RetiInstruction(instructionPosition));
                    else if ((instruction & 0xF0) == 0x80)
                        _logger?.Fatal("Unimplemented Instructiontype 'SLEEP'.");
                    else if ((instruction & 0xF0) == 0x90)
                        _logger?.Fatal("Unimplemented Instructiontype 'BREAK'.");
                    else if ((instruction & 0xF0) == 0xA0)
                        _logger?.Fatal("Unimplemented Instructiontype 'WDR'.");
                    else if ((instruction & 0xF0) == 0xC0)
                        _logger?.Fatal("Unimplemented Instructiontype 'LPM'.");
                    else if ((instruction & 0xF0) == 0xD0)
                        _logger?.Fatal("Unimplemented Instructiontype 'ELPM'.");
                    else if ((instruction & 0xF0) == 0xE0)
                        _logger?.Fatal("Unimplemented Instructiontype 'SPM'.");
                    else if ((instruction & 0xF0) == 0xF0)
                        _logger?.Fatal("Unimplemented Instructiontype 'SPM Z+'.");
                }
                else if ((instruction & 0xFE0F) >= 0x9409)
                {
                    if ((instruction & 0xF) == 0x9)
                    {
                        if ((instruction & 0x10) != 0)
                            _logger?.Fatal("Unimplemented Instructiontype 'EIJMP' and 'EICALL'.");

                        if ((instruction & 0x100) == 0)
                            Instructions.Add(new IjmpInstructioncs(instructionPosition, (instruction & 0x10) != 0));
                        else
                            Instructions.Add(new IcallInstruction(instructionPosition, (instruction & 0x10) != 0));
                    }
                    else if ((instruction & 0xF) == 0xA)
                        Instructions.Add(new DecInstruction(instructionPosition, (instruction & 0x1F0) >> 4));
                    else if ((instruction & 0xF) == 0xB)
                        _logger?.Fatal("Unimplemented Instructiontype 'DES k'.");
                    else if ((instruction & 0xC) == 0xC)
                    {
                        if ((instruction & 0x2) == 0)
                            Instructions.Add(new JmpInstruction(instructionPosition, ((instruction & 0x1F0) << 13) | ((instruction & 1) << 12) | ReadUInt16(file)));
                        else
                            Instructions.Add(new CallInstruction(instructionPosition, ((instruction & 0x1F0) << 13) | ((instruction & 1) << 12) | ReadUInt16(file)));
                    }
                }
                else if ((instruction & 0xFE00) == 0x9600)
                {
                    // ADIW and SBIW
                }
                else if ((instruction & 0xFC00) == 0x9800)
                {
                    // CBI and SBIC
                }
                else if ((instruction & 0xFC00) == 0x9C00)
                {
                    // MUL
                }
                else if ((instruction & 0xE000) >= 0xA000)
                {
                    if ((instruction & 0xE000) == 0xC000)
                        if ((instruction & 0x1000) == 0)
                            Instructions.Add(new RjmpInstruction(instructionPosition, (short)(((instruction & 0x800) == 0 ? 0 : 0xF000) | (instruction & 0xFFF))));
                        else
                            Instructions.Add(new RcallInstruction(instructionPosition, (short)(((instruction & 0x800) == 0 ? 0 : 0xF000) | (instruction & 0xFFF))));
                    else if ((instruction & 0xF000) == 0xE000)
                        Instructions.Add(new LdiInstruction(instructionPosition, (instruction & 0xF0) >> 4, (byte)(((instruction & 0xF00) >> 4) | (instruction & 0xF))));
                }
                else
                {
                    throw new UndefinedInstructionException(instruction, instructionPosition);
                }
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
