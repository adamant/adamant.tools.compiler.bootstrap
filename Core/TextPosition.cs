using System;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public readonly struct TextPosition : IEquatable<TextPosition>, IComparable<TextPosition>
    {
        public int CharacterOffset { get; } // Zero based
        public int Line { get; } // One based
        public int Column { get; } // One based

        public TextPosition(int characterOffset, int line, int column)
        {
            CharacterOffset = characterOffset;
            Line = line;
            Column = column;
        }

        public override bool Equals(object? obj)
        {
            if (obj is TextPosition other) return CharacterOffset == other.CharacterOffset;
            return false;
        }

        public bool Equals(TextPosition other)
        {
            return CharacterOffset == other.CharacterOffset;
        }

        public int CompareTo(TextPosition other)
        {
            return CharacterOffset.CompareTo(other.CharacterOffset);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CharacterOffset);
        }

        public static bool operator ==(TextPosition left, TextPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextPosition left, TextPosition right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(TextPosition left, TextPosition right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(TextPosition left, TextPosition right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(TextPosition left, TextPosition right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(TextPosition left, TextPosition right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
