using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    [Closed(
        typeof(IStringLiteralToken),
        typeof(IKeywordToken),
        typeof(IIdentifierOrUnderscoreToken),
        typeof(IOperatorToken),
        typeof(ILiteralToken),
        typeof(ILifetimeNameToken),
        typeof(IPrimitiveTypeToken),
        typeof(IBinaryOperatorToken),
        typeof(IPunctuationToken),
        typeof(IEndOfFileToken))]
    public partial interface IEssentialToken : IToken
    {
    }
}
