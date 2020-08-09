using System;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    [Closed(
        typeof(BoolType),
        typeof(NumericType))]
    public abstract class SimpleType : ValueType
    {
        public SpecialTypeName Name { get; }

        public override TypeSemantics Semantics => TypeSemantics.Copy;

        private protected SimpleType(SpecialTypeName name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.ToString();
        }

        public override bool Equals(DataType? other)
        {
            // Most simple types are fixed instances, so a reference comparision suffices
            return ReferenceEquals(this, other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
