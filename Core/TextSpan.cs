using System;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    [DebuggerDisplay("positions {Start} to {End}")]
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

        [System.Diagnostics.Contracts.Pure]
        public static TextSpan FromStartEnd(int start, int end)
        {
            Requires.Positive(nameof(start), start);
            Requires.Positive(nameof(end), end);
            return new TextSpan(start, end - start);
        }

        [System.Diagnostics.Contracts.Pure]
        public static TextSpan Covering(TextSpan x, TextSpan y)
        {
            return FromStartEnd(Math.Min(x.Start, y.Start), Math.Max(x.End, y.End));
        }

        [System.Diagnostics.Contracts.Pure]
        public static TextSpan Covering(params TextSpan?[] spans)
        {
            return spans.Where(s => s != null).Select(s => s.Value).Aggregate(Covering);
        }

        /// <summary>
        /// Returns a zero length span that occurs at the start of the current span
        /// </summary>
        public TextSpan AtStart()
        {
            return new TextSpan(Start, 0);
        }

        /// <summary>
        /// Returns a zero length span that occurs at the end of the current span
        /// </summary>
        [System.Diagnostics.Contracts.Pure]
        public TextSpan AtEnd()
        {
            return new TextSpan(End, 0);
        }

        [System.Diagnostics.Contracts.Pure]

        public string GetText(string text)
        {
            return text.Substring(Start, Length);
        }

        #region Equality
        [System.Diagnostics.Contracts.Pure]
        public override bool Equals(object obj)
        {
            return obj is TextSpan span && Equals(span);
        }

        [System.Diagnostics.Contracts.Pure]
        public bool Equals(TextSpan other)
        {
            return Start == other.Start &&
                   Length == other.Length;
        }

        [System.Diagnostics.Contracts.Pure]
        public override int GetHashCode()
        {
            return HashCode.Combine(Start, Length);
        }

        [System.Diagnostics.Contracts.Pure]
        public static bool operator ==(TextSpan span1, TextSpan span2)
        {
            return span1.Equals(span2);
        }

        [System.Diagnostics.Contracts.Pure]
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
            var comparison = Start.CompareTo(other.Start);
            return comparison != 0 ? comparison : Length.CompareTo(other.Length);
        }
    }
}
