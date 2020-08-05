using System;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen.Config
{
    public class GrammarSymbol : IEquatable<GrammarSymbol>
    {
        public string Text { get; }
        public bool IsQuoted { get; }

        public GrammarSymbol(string text, bool isQuoted = false)
        {
            Text = text;
            IsQuoted = isQuoted;
        }

        public bool Equals(GrammarSymbol? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Text == other.Text && IsQuoted == other.IsQuoted;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as GrammarSymbol);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, IsQuoted);
        }

        public static bool operator ==(GrammarSymbol? left, GrammarSymbol? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GrammarSymbol? left, GrammarSymbol? right)
        {
            return !Equals(left, right);
        }
    }
}