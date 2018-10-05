using System;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    /// The text of a source code file
    public class CodeText
    {
        public readonly string Text;
        public int Length => Text.Length;
        public TextLines Lines => lines.Value;
        private readonly Lazy<TextLines> lines;

        public string this[TextSpan span] => Text.Substring(span.Start, span.Length);
        public char this[int index] => Text[index];

        public CodeText(string text)
        {
            Requires.NotNull(nameof(text), text);
            Text = text;
            lines = new Lazy<TextLines>(GetLines);
        }

        private TextLines GetLines()
        {
            return new TextLines(Text);
        }

        public TextPosition PositionOfStart(in TextSpan span)
        {
            var charOffset = span.Start;
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
