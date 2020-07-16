using System;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    [Closed(
        typeof(ParentSymbol),
        typeof(BindingSymbol))]
    public abstract class Symbol : IEquatable<Symbol>
    {
        public Name FullName { get; }

        public bool IsGlobal => FullName is SimpleName;

        protected Symbol(Name fullName)
        {
            // Note: constructor can't be `private protected` so `Symbol` can be mocked in unit tests
            FullName = fullName;
        }

        // TODO lookup method
        public bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return FullName.Equals(other.FullName);
            //&& AttributesEqual(other);
        }

        public sealed override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Symbol)obj);
        }

        // TODO protected abstract bool AttributesEqual(Symbol other);

        public override int GetHashCode()
        {
            return HashCode.Combine(FullName);
        }
    }
}
