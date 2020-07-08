using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Core
{
    [Trait("Category", "UnitTest")]
    public class TextLinesTests
    {
        public const string OneLine = "Line0";
        public const string TwoLines = "Line0\r\nLine1";
        public const string NewlineCarriageReturn = "Line0\n\rLine1";
        public const string AllLineEndings = "Line0\rLine1\r\nLine2\nLine3\x0Bline4\fline5\x85line6\u2028line7\u2029line8";
        public const string EndsInNewline = "Line0\n";

        [Theory]
        [InlineData(OneLine, 0, 0)]
        [InlineData(OneLine, 3, 0)]
        [InlineData(OneLine, 4, 0)] // Last char
        [InlineData(OneLine, 5, 0)] // End
        [InlineData(TwoLines, 0, 0)]
        [InlineData(TwoLines, 4, 0)] // Last char of line 0
        [InlineData(TwoLines, 5, 0)] // \r
        [InlineData(TwoLines, 6, 0)] // \n
        [InlineData(TwoLines, 7, 1)]
        [InlineData(TwoLines, 11, 1)] // Last char of line 1
        [InlineData(TwoLines, 12, 1)] // End
        public void LineIndexContainingOffsetInText(string text, int charOffset, int expectedLine)
        {
            var lines = new TextLines(text);

            var line = lines.LineIndexContainingOffset(charOffset);

            Assert.Equal(expectedLine, line);
        }

        [Theory]
        [InlineData(OneLine)]
        [InlineData(TwoLines)]
        [InlineData(NewlineCarriageReturn)]
        [InlineData(AllLineEndings)]
        [InlineData(EndsInNewline)]
        public void LineIndexContainingOffsetOutsideOfText(string text)
        {
            var lines = new TextLines(text);

            Assert.Throws<ArgumentOutOfRangeException>(() => lines.LineIndexContainingOffset(-5));
            Assert.Throws<ArgumentOutOfRangeException>(() => lines.LineIndexContainingOffset(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => lines.LineIndexContainingOffset(text.Length + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => lines.LineIndexContainingOffset(text.Length + 42));
        }


        [Theory]
        [InlineData(OneLine, 1)]
        [InlineData(TwoLines, 2)]
        [InlineData(NewlineCarriageReturn, 3)]
        [InlineData(AllLineEndings, 9)]
        [InlineData(EndsInNewline, 2)]
        public void LineCount(string text, int expectedLineCount)
        {
            var lines = new TextLines(text);

            var lineCount = lines.Count;

            Assert.Equal(expectedLineCount, lineCount);
        }
    }
}
