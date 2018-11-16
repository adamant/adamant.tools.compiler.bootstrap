using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    /// <summary>
    /// A non-missing token
    /// </summary>
    public interface IToken
    {
        TextSpan Span { get; }

        [Pure]
        [NotNull]
        string Text([NotNull] CodeText code);
    }
}
