using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace assembly_aarch32.Exceptions
{
    [Serializable]
    public class UnknownConditionException : Exception
    {
        public UnknownConditionException(byte condition) : base($"Unknown condition 0x{condition:X2}.")
        {
            Data.Add("Condition", condition);
        }

        public UnknownConditionException(string message) : base(message)
        {
        }

        public UnknownConditionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnknownConditionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
