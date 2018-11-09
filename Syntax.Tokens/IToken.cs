using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    /// <summary>
    /// While <see cref="Token"/> can't be missing, <see cref="IToken"/> can be
    /// a <see cref="MissingToken"/>.
    /// </summary>
    public interface IToken : ISyntaxNodeOrToken
    {
        TextSpan Span { get; }
    }
}
