namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface IOperatorToken : IToken, IOperatorTokenPlace { }

    public partial interface IDotToken : IOperatorToken { }
    public partial interface IDotDotToken : IOperatorToken { }
    public partial interface ILessThanDotDotToken : IOperatorToken { }
    public partial interface IDotDotLessThanToken : IOperatorToken { }
    public partial interface ILessThanDotDotLessThanToken : IOperatorToken { }
    public partial interface IAtSignToken : IOperatorToken { }
    public partial interface ICaretToken : IOperatorToken { }
    public partial interface ICaretDotToken : IOperatorToken { }
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
    public partial interface IDollarToken : IOperatorToken { }
    public partial interface IDollarLessThanToken : IOperatorToken { }
    public partial interface IDollarLessThanNotEqualToken : IOperatorToken { }
    public partial interface IDollarGreaterThanToken : IOperatorToken { }
    public partial interface IDollarGreaterThanNotEqualToken : IOperatorToken { }
    public partial interface IQuestionToken : IOperatorToken { }
    public partial interface IQuestionQuestionToken : IOperatorToken { }
    public partial interface IQuestionDotToken : IOperatorToken { }
    public partial interface ILessThanColonToken : IOperatorToken { }
    public partial interface IEqualsGreaterThanToken : IOperatorToken { }
    public partial interface IAndKeywordToken : IOperatorToken { }
    public partial interface IOrKeywordToken : IOperatorToken { }
    public partial interface IXorKeywordToken : IOperatorToken { }
    public partial interface INotKeywordToken : IOperatorToken { }
}
