using System;

namespace CpuContract.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UniqueIdentifierAttribute : Attribute
    {
        public string UniqueIdentifier { get; }

        public UniqueIdentifierAttribute(string uniqueIdentifier)
        {
            UniqueIdentifier = uniqueIdentifier;
        }
    }
}
