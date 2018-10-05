using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public interface IToken
    {
        TokenKind Kind { get; }
        bool IsMissing { get; }
        TextSpan Span { get; }
        object Value { get; }
    }
}
