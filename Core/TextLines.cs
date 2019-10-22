using System;
using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public class TextLines
    {
        public int Count => startOfLine.Count;
        public IReadOnlyList<int> StartOfLine { get; }
        // Keep the original list since read only list doesn't have binary search
        private readonly List<int> startOfLine;
        private readonly int textLength;

        public TextLines(string text)
        {
            textLength = text.Length;
            startOfLine = LineStarts(text);
            StartOfLine = startOfLine.AsReadOnly();
        }

        private static List<int> LineStarts(string text)
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
                         || c == '\x0B' // vertical tab
                         || c == '\f'
                         || c == '\x85' // next line
                         || c == '\u2028' // line separator
                         || c == '\u2029') // paragraph separator
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
            // Start is allowed to be equal to length to allow for a zero length span after the last character
            if (charOffset < 0 || charOffset > textLength)
                throw new ArgumentOutOfRangeException(nameof(charOffset), charOffset,
                    $"Character offset not in text lines of length {textLength}");

            var searchResult = startOfLine.BinarySearch(charOffset);
            if (searchResult < 0)
            {
                // Not found, bitwise complement of next larger element
                return ~searchResult - 1;
            }
            return searchResult;
        }
    }
}
