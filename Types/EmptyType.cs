using System;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    /// <summary>
    /// The two empty types are `never` and `void`. They are the only types with
    /// no values.
    /// </summary>
    [Closed(
        typeof(VoidType),
        typeof(NeverType))]
    public abstract class EmptyType : DataType
    {
        public SpecialTypeName Name { get; }

        public override bool IsEmpty => true;

        public override bool IsKnown => true;

        private protected EmptyType(SpecialTypeName name)
        {
            Name = name;
        }

        public override string ToSourceCodeString()
        {
            return Name.ToString();
        }

        public override string ToILString()
        {
            return ToSourceCodeString();
        }

        public override bool Equals(DataType? other)
        {
            // Empty types are all fixed instances, so a reference equality suffices
            return ReferenceEquals(this, other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
