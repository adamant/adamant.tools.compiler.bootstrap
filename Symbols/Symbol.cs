using System;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    // TODO have symbols reference their containing symbols and act a little more like names
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

        #region Equality
        public abstract bool Equals(Symbol? other);

        public abstract override int GetHashCode();

        public sealed override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Symbol)obj);
        }

        public static bool operator ==(Symbol? symbol1, Symbol? symbol2)
        {
            return Equals(symbol1, symbol2);
        }

        public static bool operator !=(Symbol? symbol1, Symbol? symbol2)
        {
            return !(symbol1 == symbol2);
        }
        #endregion
    }
}
