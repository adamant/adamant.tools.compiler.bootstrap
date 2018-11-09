using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    /// <summary>
    /// An <see cref="IToken"/> or <see cref="IMissingToken"/>
    /// </summary>
    public interface ITokenPlace
    {
        TextSpan Span { get; }
    }
}
