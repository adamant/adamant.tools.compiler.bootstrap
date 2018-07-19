namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    /// The text of a source code file
    public class CodeText
    {
        public readonly string Text;
        public int Length => Text.Length;
        public readonly TextLines Lines;

        public string this[TextSpan span] => Text.Substring(span.Start, span.Length);
        public char this[int index] => Text[index];

        public CodeText(string text)
        {
            Text = text;
        }
    }
}
