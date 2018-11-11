using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public class TextLines
    {
        [NotNull] public readonly string Text; // TODO don't keep a reference to the whole string, we only need the length
        public int Count => startOfLine.Count;
        [NotNull] public readonly IReadOnlyList<int> StartOfLine;
        [NotNull] private readonly List<int> startOfLine;

        public TextLines([NotNull] string text)
        {
            Requires.NotNull(nameof(text), text);
            Text = text;
            startOfLine = LineStarts(text);
            StartOfLine = startOfLine.AsReadOnly().NotNull();
        }

        [NotNull]
        private static List<int> LineStarts([NotNull] string text)
        {
            var length = text.Length;
            var startOfLine = new List<int> { 0 }; // there is always a first line

            // performance critical
            var position = 0;
            while (position < length)
            {
                var c = text[position];
                position += 1;

                // Common case - ASCII & not a line break
                if (c > '\r' && c <= '\x7F') continue;

                if (c == '\r')
                {
                    // Assumes that the only 2-char line break sequence is CR+LF
                    if (position < length && text[position] == '\n')
                        position += 1;
                }
                else if (c == '\n'
                         || c == '\x0B'
                         || c == '\f'
                         || c == '\x85')
                // || c == '\u2028'
                // || c == '\u2029'
                {
                    // Do Nothing
                }
                else continue;

                // Already advanced position, so next line starts there
                startOfLine.Add(position);
            }

            return startOfLine;
        }

        /// <summary>
        /// Returns zero based line index
        /// </summary>
        public int LineIndexContainingOffset(int charOffset)
        {
            Requires.InString(Text, nameof(charOffset), charOffset);
            var searchResult = startOfLine.BinarySearch(charOffset);
            // TODO not sure this is right
            if (searchResult < 0)
            {
                // Not found, bitwise complement of next larger element
                return ~searchResult - 1;
            }
            return searchResult;
        }
    }
}
