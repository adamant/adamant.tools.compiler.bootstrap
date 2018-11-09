using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    /// <summary>
    /// An <see cref="ITokenPlace"/> or <see cref="IMissingToken"/>
    /// </summary>
    public interface ITokenPlace : ISyntaxNodeOrTokenPlace
    {
        TextSpan Span { get; }

        [Pure]
        [NotNull]
        string Text([NotNull] CodeText code);
    }
}
