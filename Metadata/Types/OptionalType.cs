using System;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// The type `T?` is an optional type. Optional types either have the value
    /// `none` or a value of their referent type. The referent type may be a value
    /// type or a reference type. Optional types themselves are always immutable.
    /// However the referent type may be mutable or immutable. Effectively, optional
    /// types are like an immutable struct type `Optional[T]`. However, the value
    /// semantics are strange. They depend on the referent type.
    /// </summary>
    public class OptionalType : ValueType, IEquatable<OptionalType>
    {
        public DataType Referent { get; }

        public override bool IsKnown { get; }

        public override ValueSemantics ValueSemantics => Referent.ValueSemantics;

        public OptionalType(DataType referent)
        {
            Referent = referent;
            IsKnown = referent.IsKnown;
        }



        public override string ToString()
        {
            return $"({Referent})?";
        }

        public override bool Equals(object? other)
        {
            return Equals(other as OptionalType);
        }

        public bool Equals(OptionalType? other)
        {
            if (other is null) return false;
            return ReferenceEquals(this, other)
                || Equals(Referent, other.Referent);
        }

        public override int GetHashCode()
        {
            return (Referent != null ? Referent.GetHashCode() : 0);
        }
    }
}
