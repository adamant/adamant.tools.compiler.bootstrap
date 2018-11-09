using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    internal abstract partial class Token : IToken
    {
        public TextSpan Span { get; }

        protected Token(TextSpan span)
        {
            Span = span;
        }

        [Pure]
        [NotNull]
        public string Text([NotNull] CodeText code)
        {
            return Span.GetText(code.Text);
        }
    }
}
