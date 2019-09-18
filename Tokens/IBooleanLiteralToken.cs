using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    [Closed(
        typeof(ITrueKeywordToken),
        typeof(IFalseKeywordToken))]
    public partial interface IBooleanLiteralToken : IKeywordToken
    {
        bool Value { get; }
    }

    public partial interface ITrueKeywordToken : IBooleanLiteralToken { }
    public partial interface IFalseKeywordToken : IBooleanLiteralToken { }
}
