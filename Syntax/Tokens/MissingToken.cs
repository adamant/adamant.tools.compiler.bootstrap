using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public class MissingToken
    {
        public readonly TextSpan Span;

        public MissingToken(TextSpan span)
        {
            Span = span;
        }

        [System.Diagnostics.Contracts.Pure]
        [NotNull]
        public string Text([NotNull] CodeText code)
        {
            return Span.GetText(code.Text);
        }
    }
}
