using System;

namespace CpuContract.Attributes
{
    /// <summary>
    /// An attribute identifying classes to an architecture.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UniqueIdentifierAttribute : Attribute
    {
        /// <summary>
        /// The identifier for the architecture.
        /// </summary>
        public string UniqueIdentifier { get; }

        public UniqueIdentifierAttribute(string uniqueIdentifier)
        {
            UniqueIdentifier = uniqueIdentifier;
        }
    }
}
