using System;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen.Config
{
    public sealed class GrammarType : IEquatable<GrammarType>
    {
        public GrammarSymbol Symbol { get; }
        public bool IsOptional { get; }
        public bool IsList { get; }

        public GrammarType(GrammarSymbol symbol, bool isOptional, bool isList)
        {
            Symbol = symbol;
            IsOptional = isOptional;
            IsList = isList;
        }

        public bool Equals(GrammarType? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Symbol.Equals(other.Symbol)
                   && IsOptional == other.IsOptional
                   && IsList == other.IsList;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is GrammarType other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Symbol, IsOptional, IsList);
        }

        public static bool operator ==(GrammarType? left, GrammarType? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GrammarType? left, GrammarType? right)
        {
            return !Equals(left, right);
        }
    }
}
