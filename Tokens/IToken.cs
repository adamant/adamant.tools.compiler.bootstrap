using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    /// <summary>
    /// A non-missing token
    /// </summary>
    public interface IToken : ISyntaxNodeOrToken
    {
        TextSpan Span { get; }
    }
}
