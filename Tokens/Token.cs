using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    internal abstract class Token
    {
        public TextSpan Span { get; }

        protected Token(TextSpan span)
        {
            Span = span;
        }
        public string Text(CodeText code)
        {
            return Span.GetText(code.Text);
        }
    }
}
