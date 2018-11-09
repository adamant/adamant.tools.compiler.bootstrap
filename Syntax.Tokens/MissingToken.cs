using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public partial class MissingToken : IToken, IIdentifierToken
    {
        public TextSpan Span { get; }

        public MissingToken(TextSpan span)
        {
            Span = span;
        }

        [Pure]
        [NotNull]
        public string Text([NotNull] CodeText code)
        {
            return Span.GetText(code.Text);
        }

        [CanBeNull] string IIdentifierToken.Value => null;
    }
}
