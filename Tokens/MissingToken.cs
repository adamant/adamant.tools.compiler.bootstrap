using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    internal class MissingToken : IMissingToken, IIdentifierTokenPlace
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

        [CanBeNull] string IIdentifierTokenPlace.Value => null;
    }

    public static partial class TokenFactory
    {
        [NotNull]
        public static IMissingToken Missing(TextSpan span)
        {
            return new MissingToken(span);
        }
    }
}
