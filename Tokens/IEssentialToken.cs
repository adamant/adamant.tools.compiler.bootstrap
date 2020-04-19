using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    /// <summary>
    /// A token that isn't trivia
    /// </summary>
    [Closed(
        typeof(IStringLiteralToken),
        typeof(IKeywordToken),
        typeof(IIdentifierOrUnderscoreToken),
        typeof(IOperatorToken),
        typeof(ILiteralToken),
        typeof(IPrimitiveTypeToken),
        typeof(IBinaryOperatorToken),
        typeof(IPunctuationToken),
        typeof(IEndOfFileToken))]
    public partial interface IEssentialToken : IToken
    {
    }
}
