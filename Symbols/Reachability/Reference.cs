using System;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Reachability
{
    public abstract class Reference : IEquatable<Reference>
    {
        public abstract bool Equals(Reference? other);

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Reference)obj);
        }

        public abstract override int GetHashCode();

        public static bool operator ==(Reference? left, Reference? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Reference? left, Reference? right)
        {
            return !Equals(left, right);
        }
    }
}
