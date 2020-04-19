using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    [Closed(
        typeof(IDotDotToken),
        typeof(ILessThanDotDotToken),
        typeof(IDotDotLessThanToken),
        typeof(ILessThanDotDotLessThanToken),
        typeof(IPlusToken),
        typeof(IMinusToken),
        typeof(IAsteriskToken),
        typeof(ISlashToken),
        typeof(IEqualsEqualsToken),
        typeof(INotEqualToken),
        typeof(IGreaterThanToken),
        typeof(IGreaterThanOrEqualToken),
        typeof(ILessThanToken),
        typeof(ILessThanOrEqualToken),
        typeof(IQuestionQuestionToken),
        typeof(IAndKeywordToken),
        typeof(IOrKeywordToken))]
    public interface IBinaryOperatorToken : IEssentialToken
    {
    }

    public partial interface IDotDotToken : IBinaryOperatorToken { }
    public partial interface ILessThanDotDotToken : IBinaryOperatorToken { }
    public partial interface IDotDotLessThanToken : IBinaryOperatorToken { }
    public partial interface ILessThanDotDotLessThanToken : IBinaryOperatorToken { }
    public partial interface IPlusToken : IBinaryOperatorToken { }
    public partial interface IMinusToken : IBinaryOperatorToken { }
    public partial interface IAsteriskToken : IBinaryOperatorToken { }
    public partial interface ISlashToken : IBinaryOperatorToken { }
    public partial interface IEqualsEqualsToken : IBinaryOperatorToken { }
    public partial interface INotEqualToken : IBinaryOperatorToken { }
    public partial interface IGreaterThanToken : IBinaryOperatorToken { }
    public partial interface IGreaterThanOrEqualToken : IBinaryOperatorToken { }
    public partial interface ILessThanToken : IBinaryOperatorToken { }
    public partial interface ILessThanOrEqualToken : IBinaryOperatorToken { }
    public partial interface IQuestionQuestionToken : IBinaryOperatorToken { }
    public partial interface IAndKeywordToken : IBinaryOperatorToken { }
    public partial interface IOrKeywordToken : IBinaryOperatorToken { }
}
