using System;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen.Config
{
    public class Symbol : IEquatable<Symbol>
    {
        public string Text { get; }
        public bool IsQuoted { get; }

        public Symbol(string text, bool isQuoted = false)
        {
            Text = text;
            IsQuoted = isQuoted;
        }

        public bool Equals(Symbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Text == other.Text && IsQuoted == other.IsQuoted;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Symbol);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, IsQuoted);
        }

        public static bool operator ==(Symbol? left, Symbol? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Symbol? left, Symbol? right)
        {
            return !Equals(left, right);
        }
    }
}
