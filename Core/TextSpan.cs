using System;
using System.Diagnostics.Contracts;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public readonly struct TextSpan : IEquatable<TextSpan>, IComparable<TextSpan>
    {
        public readonly int Start;
        public int End => Start + Length;
        public readonly int Length;
        public bool IsEmpty => Length == 0;

        public TextSpan(int start, int length)
        {
            Requires.Positive(nameof(start), start);
            Requires.Positive(nameof(length), length);
            Start = start;
            Length = length;
        }

        [Pure]
        public static TextSpan FromStartEnd(int start, int end)
        {
            Requires.Positive(nameof(start), start);
            Requires.Positive(nameof(end), end);
            return new TextSpan(start, end - start);
        }

        [Pure]
        public static TextSpan Covering(TextSpan x, TextSpan y)
        {
            return FromStartEnd(Math.Min(x.Start, y.Start), Math.Max(x.End, y.End));
        }

        [Pure]
        public string GetText(string text)
        {
            return text.Substring(Start, Length);
        }

        #region Equality
        [Pure]
        public override bool Equals(object obj)
        {
            return obj is TextSpan span && Equals(span);
        }

        [Pure]
        public bool Equals(TextSpan other)
        {
            return Start == other.Start &&
                   Length == other.Length;
        }

        [Pure]
        public override int GetHashCode()
        {
            return HashCode.Combine(Start, Length);
        }

        [Pure]
        public static bool operator ==(TextSpan span1, TextSpan span2)
        {
            return span1.Equals(span2);
        }

        [Pure]
        public static bool operator !=(TextSpan span1, TextSpan span2)
        {
            return !(span1 == span2);
        }
        #endregion

        public override string ToString()
        {
            return $"TextSpan({Start},{Length})";
        }

        public int CompareTo(TextSpan other)
        {
            var startComparison = Start.CompareTo(other.Start);
            return startComparison != 0 ? startComparison : Length.CompareTo(other.Length);
        }
    }
}
