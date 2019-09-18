using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    /// <summary>
    /// A non-missing token
    /// </summary>
    [Closed(
        typeof(IStringLiteralToken),
        typeof(IKeywordToken),
        typeof(IIdentifierOrUnderscoreToken),
        typeof(IOperatorToken),
        typeof(ILiteralToken),
        typeof(ITriviaToken),
        typeof(ILifetimeNameToken),
        typeof(IPrimitiveTypeToken),
        typeof(IIntegerLiteralToken))]
    public partial interface IToken
    {
        TextSpan Span { get; }

        string Text(CodeText code);
    }
}
