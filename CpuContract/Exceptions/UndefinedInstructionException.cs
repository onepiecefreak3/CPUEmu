using System;
using System.Runtime.Serialization;

namespace CpuContract.Exceptions
{
    [Serializable]
    public class UndefinedInstructionException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public UndefinedInstructionException(uint instruction, int position) : base($"Undefined instruction 0x{instruction:X8} at position 0x{position:X8}.")
        {
            Data.Add("Instruction", instruction);
            Data.Add("Position", position);
        }

        public UndefinedInstructionException(string message) : base(message)
        {
        }

        public UndefinedInstructionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UndefinedInstructionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
