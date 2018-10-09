using System;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    /// The text of a source code file
    public class CodeText
    {
        [NotNull]
        public readonly string Text;

        [NotNull]
        public int Length => Text.Length;

        [NotNull]
        public TextLines Lines => lines.Value;

        [NotNull]
        private readonly Lazy<TextLines> lines;

        public string this[TextSpan span] => Text.Substring(span.Start, span.Length);
        public char this[int index] => Text[index];

        public CodeText([NotNull] string text)
        {
            Requires.NotNull(nameof(text), text);
            Text = text;
            lines = new Lazy<TextLines>(GetLines);
        }

        [NotNull]
        private TextLines GetLines()
        {
            return new TextLines(Text);
        }

        public TextPosition PositionOfStart(in TextSpan span)
        {
            return PositionOf(span.Start);
        }

        public TextPosition PositionOfEnd(in TextSpan span)
        {
            if (span.IsEmpty)
                return PositionOfStart(span);
            // End is one past, we want the actual last char
            return PositionOf(span.End - 1);
        }

        private TextPosition PositionOf(int charOffset)
        {
            var lineNumber = Lines.LineContainingOffset(charOffset);
            var lineStart = Lines.StartOfLine[lineNumber];

            // TODO handle Unicode
            var column = charOffset - lineStart + 1; // column is one based
            // Account for tabs being multiple columns
            // TODO switch to a for loop when we have range expressions
            var i = lineStart;
            while (i < charOffset)
            {
                if (Text[i] == '\t')
                    column += 3;  // tabs are 4 columns, but the character was already counted as 1
                i += 1;
            }

            // The line number is one based while the variable was zero based
            return new TextPosition(charOffset, lineNumber + 1, column);
        }
    }
}
