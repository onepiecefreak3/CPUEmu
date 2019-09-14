using System;
using System.Runtime.Serialization;

namespace assembly_aarch32.Exceptions
{
    [Serializable]
    public class UnknownShiftTypeException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public UnknownShiftTypeException(int shiftType) : base($"Unknown shift type 0x{shiftType:X1} in barrel shifter.")
        {
            Data.Add("ShiftType", shiftType);
        }

        public UnknownShiftTypeException(string message) : base(message)
        {
        }

        public UnknownShiftTypeException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnknownShiftTypeException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
