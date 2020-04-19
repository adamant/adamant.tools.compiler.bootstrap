using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    [Closed(
        typeof(ILifetimeOperatorToken),
        typeof(IAccessOperatorToken),
        typeof(IDotToken),
        typeof(IDotDotToken),
        typeof(ILessThanDotDotToken),
        typeof(IDotDotLessThanToken),
        typeof(ILessThanDotDotLessThanToken),
        typeof(IPlusToken),
        typeof(IMinusToken),
        typeof(IAsteriskToken),
        typeof(ISlashToken),
        typeof(IEqualsToken),
        typeof(IEqualsEqualsToken),
        typeof(INotEqualToken),
        typeof(IGreaterThanToken),
        typeof(IGreaterThanOrEqualToken),
        typeof(ILessThanToken),
        typeof(ILessThanOrEqualToken),
        typeof(IPlusEqualsToken),
        typeof(IMinusEqualsToken),
        typeof(IAsteriskEqualsToken),
        typeof(ISlashEqualsToken),
        typeof(IQuestionToken),
        typeof(IQuestionQuestionToken),
        typeof(IQuestionDotToken),
        typeof(ILessThanColonToken),
        typeof(IRightDoubleArrowToken),
        typeof(ILeftWaveArrowToken),
        typeof(IRightWaveArrowToken),
        typeof(IAndKeywordToken),
        typeof(IOrKeywordToken),
        typeof(INotKeywordToken),
        typeof(IAssignmentToken))]
    public interface IOperatorToken : IEssentialToken { }

    public partial interface IDotToken : IOperatorToken { }
    public partial interface IDotDotToken : IOperatorToken { }
    public partial interface ILessThanDotDotToken : IOperatorToken { }
    public partial interface IDotDotLessThanToken : IOperatorToken { }
    public partial interface ILessThanDotDotLessThanToken : IOperatorToken { }
    public partial interface IPlusToken : IOperatorToken { }
    public partial interface IMinusToken : IOperatorToken { }
    public partial interface IAsteriskToken : IOperatorToken { }
    public partial interface ISlashToken : IOperatorToken { }
    public partial interface IEqualsToken : IOperatorToken { }
    public partial interface IEqualsEqualsToken : IOperatorToken { }
    public partial interface INotEqualToken : IOperatorToken { }
    public partial interface IGreaterThanToken : IOperatorToken { }
    public partial interface IGreaterThanOrEqualToken : IOperatorToken { }
    public partial interface ILessThanToken : IOperatorToken { }
    public partial interface ILessThanOrEqualToken : IOperatorToken { }
    public partial interface IPlusEqualsToken : IOperatorToken { }
    public partial interface IMinusEqualsToken : IOperatorToken { }
    public partial interface IAsteriskEqualsToken : IOperatorToken { }
    public partial interface ISlashEqualsToken : IOperatorToken { }
    public partial interface IQuestionToken : IOperatorToken { }
    public partial interface IQuestionQuestionToken : IOperatorToken { }
    public partial interface IQuestionDotToken : IOperatorToken { }
    public partial interface ILessThanColonToken : IOperatorToken { }
    public partial interface IRightDoubleArrowToken : IOperatorToken { }
    public partial interface ILeftWaveArrowToken : IOperatorToken { }
    public partial interface IRightWaveArrowToken : IOperatorToken { }
    public partial interface IAndKeywordToken : IOperatorToken { }
    public partial interface IOrKeywordToken : IOperatorToken { }
    public partial interface INotKeywordToken : IOperatorToken { }
}
