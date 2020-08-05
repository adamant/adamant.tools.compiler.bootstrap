using System;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    /// <summary>
    /// A name for a type. Type names may be standard names, or special names
    /// </summary>
    [Closed(
        typeof(Name),
        typeof(SpecialTypeName))]
    public abstract class TypeName : IEquatable<TypeName>
    {
        public string Text { get; }

        protected TypeName(string text)
        {
            Text = text;
        }

        public abstract bool Equals(TypeName? other);

        public override bool Equals(object? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is TypeName otherTypeName && Equals(otherTypeName);
        }

        public abstract override int GetHashCode();

        public static bool operator ==(TypeName? left, TypeName? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TypeName? left, TypeName? right)
        {
            return !Equals(left, right);
        }

        public abstract override string ToString();
    }
}
