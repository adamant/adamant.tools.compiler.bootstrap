namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public readonly struct TextSpan
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

        public static TextSpan FromStartEnd(int start, int end)
        {
            Requires.Positive(nameof(start), start);
            Requires.Positive(nameof(end), end);
            return new TextSpan(start, end - start);
        }
    }
}
