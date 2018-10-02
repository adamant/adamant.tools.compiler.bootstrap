using System;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public struct TextPosition : IComparable<TextPosition>
    {
        public readonly int CharacterOffset; // Zero based
        public readonly int Line; // One based
        public readonly int Column; // One based

        public TextPosition(int characterOffset, int line, int column)
        {
            CharacterOffset = characterOffset;
            Line = line;
            Column = column;
        }

        public int CompareTo(TextPosition other)
        {
            return CharacterOffset.CompareTo(other.CharacterOffset);
        }
    }
}
