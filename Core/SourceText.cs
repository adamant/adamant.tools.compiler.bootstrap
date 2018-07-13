namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    /// The text of a source file
    public class SourceText
    {
        public readonly string Content;
        public int Length => Content.Length;
        public readonly TextLines Lines;

        public string this[TextSpan span] => Content.Substring(span.Start, span.Length);
        public char this[int index] => Content[index];

        public SourceText(string content)
        {
            Content = content;
        }
    }
}
