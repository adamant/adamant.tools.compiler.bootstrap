using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public static partial class TokenFactory
    {
        [NotNull]
        public static IWhitespaceToken Whitespace(TextSpan span)
        {
            return new WhitespaceToken(span);
        }

        [NotNull]
        public static ICommentToken Comment(TextSpan span)
        {
            return new CommentToken(span);
        }

        [NotNull]
        public static IUnexpectedToken Unexpected(TextSpan span)
        {
            return new UnexpectedToken(span);
        }

        [NotNull]
        public static IOpenBraceToken OpenBrace(TextSpan span)
        {
            return new OpenBraceToken(span);
        }

        [NotNull]
        public static ICloseBraceToken CloseBrace(TextSpan span)
        {
            return new CloseBraceToken(span);
        }

        [NotNull]
        public static IOpenParenToken OpenParen(TextSpan span)
        {
            return new OpenParenToken(span);
        }

        [NotNull]
        public static ICloseParenToken CloseParen(TextSpan span)
        {
            return new CloseParenToken(span);
        }

        [NotNull]
        public static IOpenBracketToken OpenBracket(TextSpan span)
        {
            return new OpenBracketToken(span);
        }

        [NotNull]
        public static ICloseBracketToken CloseBracket(TextSpan span)
        {
            return new CloseBracketToken(span);
        }

        [NotNull]
        public static ISemicolonToken Semicolon(TextSpan span)
        {
            return new SemicolonToken(span);
        }

        [NotNull]
        public static ICommaToken Comma(TextSpan span)
        {
            return new CommaToken(span);
        }

        [NotNull]
        public static IPipeToken Pipe(TextSpan span)
        {
            return new PipeToken(span);
        }

        [NotNull]
        public static IColonToken Colon(TextSpan span)
        {
            return new ColonToken(span);
        }

        [NotNull]
        public static IRightArrowToken RightArrow(TextSpan span)
        {
            return new RightArrowToken(span);
        }

        [NotNull]
        public static IHashToken Hash(TextSpan span)
        {
            return new HashToken(span);
        }

        [NotNull]
        public static IHashHashToken HashHash(TextSpan span)
        {
            return new HashHashToken(span);
        }

        [NotNull]
        public static IDotToken Dot(TextSpan span)
        {
            return new DotToken(span);
        }

        [NotNull]
        public static IDotDotToken DotDot(TextSpan span)
        {
            return new DotDotToken(span);
        }

        [NotNull]
        public static ILessThanDotDotToken LessThanDotDot(TextSpan span)
        {
            return new LessThanDotDotToken(span);
        }

        [NotNull]
        public static IDotDotLessThanToken DotDotLessThan(TextSpan span)
        {
            return new DotDotLessThanToken(span);
        }

        [NotNull]
        public static ILessThanDotDotLessThanToken LessThanDotDotLessThan(TextSpan span)
        {
            return new LessThanDotDotLessThanToken(span);
        }

        [NotNull]
        public static IAtSignToken AtSign(TextSpan span)
        {
            return new AtSignToken(span);
        }

        [NotNull]
        public static ICaretToken Caret(TextSpan span)
        {
            return new CaretToken(span);
        }

        [NotNull]
        public static ICaretDotToken CaretDot(TextSpan span)
        {
            return new CaretDotToken(span);
        }

        [NotNull]
        public static IPlusToken Plus(TextSpan span)
        {
            return new PlusToken(span);
        }

        [NotNull]
        public static IMinusToken Minus(TextSpan span)
        {
            return new MinusToken(span);
        }

        [NotNull]
        public static IAsteriskToken Asterisk(TextSpan span)
        {
            return new AsteriskToken(span);
        }

        [NotNull]
        public static ISlashToken Slash(TextSpan span)
        {
            return new SlashToken(span);
        }

        [NotNull]
        public static IEqualsToken Equals(TextSpan span)
        {
            return new EqualsToken(span);
        }

        [NotNull]
        public static IEqualsEqualsToken EqualsEquals(TextSpan span)
        {
            return new EqualsEqualsToken(span);
        }

        [NotNull]
        public static INotEqualToken NotEqual(TextSpan span)
        {
            return new NotEqualToken(span);
        }

        [NotNull]
        public static IGreaterThanToken GreaterThan(TextSpan span)
        {
            return new GreaterThanToken(span);
        }

        [NotNull]
        public static IGreaterThanOrEqualToken GreaterThanOrEqual(TextSpan span)
        {
            return new GreaterThanOrEqualToken(span);
        }

        [NotNull]
        public static ILessThanToken LessThan(TextSpan span)
        {
            return new LessThanToken(span);
        }

        [NotNull]
        public static ILessThanOrEqualToken LessThanOrEqual(TextSpan span)
        {
            return new LessThanOrEqualToken(span);
        }

        [NotNull]
        public static IPlusEqualsToken PlusEquals(TextSpan span)
        {
            return new PlusEqualsToken(span);
        }

        [NotNull]
        public static IMinusEqualsToken MinusEquals(TextSpan span)
        {
            return new MinusEqualsToken(span);
        }

        [NotNull]
        public static IAsteriskEqualsToken AsteriskEquals(TextSpan span)
        {
            return new AsteriskEqualsToken(span);
        }

        [NotNull]
        public static ISlashEqualsToken SlashEquals(TextSpan span)
        {
            return new SlashEqualsToken(span);
        }

        [NotNull]
        public static IDollarToken Dollar(TextSpan span)
        {
            return new DollarToken(span);
        }

        [NotNull]
        public static IDollarLessThanToken DollarLessThan(TextSpan span)
        {
            return new DollarLessThanToken(span);
        }

        [NotNull]
        public static IDollarLessThanNotEqualToken DollarLessThanNotEqual(TextSpan span)
        {
            return new DollarLessThanNotEqualToken(span);
        }

        [NotNull]
        public static IDollarGreaterThanToken DollarGreaterThan(TextSpan span)
        {
            return new DollarGreaterThanToken(span);
        }

        [NotNull]
        public static IDollarGreaterThanNotEqualToken DollarGreaterThanNotEqual(TextSpan span)
        {
            return new DollarGreaterThanNotEqualToken(span);
        }

        [NotNull]
        public static IQuestionToken Question(TextSpan span)
        {
            return new QuestionToken(span);
        }

        [NotNull]
        public static IQuestionQuestionToken QuestionQuestion(TextSpan span)
        {
            return new QuestionQuestionToken(span);
        }

        [NotNull]
        public static IQuestionDotToken QuestionDot(TextSpan span)
        {
            return new QuestionDotToken(span);
        }

        [NotNull]
        public static ILessThanColonToken LessThanColon(TextSpan span)
        {
            return new LessThanColonToken(span);
        }

        [NotNull]
        public static IEqualsGreaterThanToken EqualsGreaterThan(TextSpan span)
        {
            return new EqualsGreaterThanToken(span);
        }

    }

    public partial interface IWhitespaceTokenPlace : ITokenPlace { }
    public partial interface IWhitespaceToken : IToken, IWhitespaceTokenPlace { }
    internal partial class WhitespaceToken : Token, IWhitespaceToken
    {
        public WhitespaceToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICommentTokenPlace : ITokenPlace { }
    public partial interface ICommentToken : IToken, ICommentTokenPlace { }
    internal partial class CommentToken : Token, ICommentToken
    {
        public CommentToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IUnexpectedTokenPlace : ITokenPlace { }
    public partial interface IUnexpectedToken : IToken, IUnexpectedTokenPlace { }
    internal partial class UnexpectedToken : Token, IUnexpectedToken
    {
        public UnexpectedToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOpenBraceTokenPlace : ITokenPlace { }
    public partial interface IOpenBraceToken : IToken, IOpenBraceTokenPlace { }
    internal partial class OpenBraceToken : Token, IOpenBraceToken
    {
        public OpenBraceToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICloseBraceTokenPlace : ITokenPlace { }
    public partial interface ICloseBraceToken : IToken, ICloseBraceTokenPlace { }
    internal partial class CloseBraceToken : Token, ICloseBraceToken
    {
        public CloseBraceToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOpenParenTokenPlace : ITokenPlace { }
    public partial interface IOpenParenToken : IToken, IOpenParenTokenPlace { }
    internal partial class OpenParenToken : Token, IOpenParenToken
    {
        public OpenParenToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICloseParenTokenPlace : ITokenPlace { }
    public partial interface ICloseParenToken : IToken, ICloseParenTokenPlace { }
    internal partial class CloseParenToken : Token, ICloseParenToken
    {
        public CloseParenToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOpenBracketTokenPlace : ITokenPlace { }
    public partial interface IOpenBracketToken : IToken, IOpenBracketTokenPlace { }
    internal partial class OpenBracketToken : Token, IOpenBracketToken
    {
        public OpenBracketToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICloseBracketTokenPlace : ITokenPlace { }
    public partial interface ICloseBracketToken : IToken, ICloseBracketTokenPlace { }
    internal partial class CloseBracketToken : Token, ICloseBracketToken
    {
        public CloseBracketToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISemicolonTokenPlace : ITokenPlace { }
    public partial interface ISemicolonToken : IToken, ISemicolonTokenPlace { }
    internal partial class SemicolonToken : Token, ISemicolonToken
    {
        public SemicolonToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICommaTokenPlace : ITokenPlace { }
    public partial interface ICommaToken : IToken, ICommaTokenPlace { }
    internal partial class CommaToken : Token, ICommaToken
    {
        public CommaToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IPipeTokenPlace : ITokenPlace { }
    public partial interface IPipeToken : IToken, IPipeTokenPlace { }
    internal partial class PipeToken : Token, IPipeToken
    {
        public PipeToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IColonTokenPlace : ITokenPlace { }
    public partial interface IColonToken : IToken, IColonTokenPlace { }
    internal partial class ColonToken : Token, IColonToken
    {
        public ColonToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IRightArrowTokenPlace : ITokenPlace { }
    public partial interface IRightArrowToken : IToken, IRightArrowTokenPlace { }
    internal partial class RightArrowToken : Token, IRightArrowToken
    {
        public RightArrowToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IHashTokenPlace : ITokenPlace { }
    public partial interface IHashToken : IToken, IHashTokenPlace { }
    internal partial class HashToken : Token, IHashToken
    {
        public HashToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IHashHashTokenPlace : ITokenPlace { }
    public partial interface IHashHashToken : IToken, IHashHashTokenPlace { }
    internal partial class HashHashToken : Token, IHashHashToken
    {
        public HashHashToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDotTokenPlace : ITokenPlace { }
    public partial interface IDotToken : IToken, IDotTokenPlace { }
    internal partial class DotToken : Token, IDotToken
    {
        public DotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDotDotTokenPlace : ITokenPlace { }
    public partial interface IDotDotToken : IToken, IDotDotTokenPlace { }
    internal partial class DotDotToken : Token, IDotDotToken
    {
        public DotDotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILessThanDotDotTokenPlace : ITokenPlace { }
    public partial interface ILessThanDotDotToken : IToken, ILessThanDotDotTokenPlace { }
    internal partial class LessThanDotDotToken : Token, ILessThanDotDotToken
    {
        public LessThanDotDotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDotDotLessThanTokenPlace : ITokenPlace { }
    public partial interface IDotDotLessThanToken : IToken, IDotDotLessThanTokenPlace { }
    internal partial class DotDotLessThanToken : Token, IDotDotLessThanToken
    {
        public DotDotLessThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILessThanDotDotLessThanTokenPlace : ITokenPlace { }
    public partial interface ILessThanDotDotLessThanToken : IToken, ILessThanDotDotLessThanTokenPlace { }
    internal partial class LessThanDotDotLessThanToken : Token, ILessThanDotDotLessThanToken
    {
        public LessThanDotDotLessThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAtSignTokenPlace : ITokenPlace { }
    public partial interface IAtSignToken : IToken, IAtSignTokenPlace { }
    internal partial class AtSignToken : Token, IAtSignToken
    {
        public AtSignToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICaretTokenPlace : ITokenPlace { }
    public partial interface ICaretToken : IToken, ICaretTokenPlace { }
    internal partial class CaretToken : Token, ICaretToken
    {
        public CaretToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICaretDotTokenPlace : ITokenPlace { }
    public partial interface ICaretDotToken : IToken, ICaretDotTokenPlace { }
    internal partial class CaretDotToken : Token, ICaretDotToken
    {
        public CaretDotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IPlusTokenPlace : ITokenPlace { }
    public partial interface IPlusToken : IToken, IPlusTokenPlace { }
    internal partial class PlusToken : Token, IPlusToken
    {
        public PlusToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMinusTokenPlace : ITokenPlace { }
    public partial interface IMinusToken : IToken, IMinusTokenPlace { }
    internal partial class MinusToken : Token, IMinusToken
    {
        public MinusToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAsteriskTokenPlace : ITokenPlace { }
    public partial interface IAsteriskToken : IToken, IAsteriskTokenPlace { }
    internal partial class AsteriskToken : Token, IAsteriskToken
    {
        public AsteriskToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISlashTokenPlace : ITokenPlace { }
    public partial interface ISlashToken : IToken, ISlashTokenPlace { }
    internal partial class SlashToken : Token, ISlashToken
    {
        public SlashToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IEqualsTokenPlace : ITokenPlace { }
    public partial interface IEqualsToken : IToken, IEqualsTokenPlace { }
    internal partial class EqualsToken : Token, IEqualsToken
    {
        public EqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IEqualsEqualsTokenPlace : ITokenPlace { }
    public partial interface IEqualsEqualsToken : IToken, IEqualsEqualsTokenPlace { }
    internal partial class EqualsEqualsToken : Token, IEqualsEqualsToken
    {
        public EqualsEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INotEqualTokenPlace : ITokenPlace { }
    public partial interface INotEqualToken : IToken, INotEqualTokenPlace { }
    internal partial class NotEqualToken : Token, INotEqualToken
    {
        public NotEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IGreaterThanTokenPlace : ITokenPlace { }
    public partial interface IGreaterThanToken : IToken, IGreaterThanTokenPlace { }
    internal partial class GreaterThanToken : Token, IGreaterThanToken
    {
        public GreaterThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IGreaterThanOrEqualTokenPlace : ITokenPlace { }
    public partial interface IGreaterThanOrEqualToken : IToken, IGreaterThanOrEqualTokenPlace { }
    internal partial class GreaterThanOrEqualToken : Token, IGreaterThanOrEqualToken
    {
        public GreaterThanOrEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILessThanTokenPlace : ITokenPlace { }
    public partial interface ILessThanToken : IToken, ILessThanTokenPlace { }
    internal partial class LessThanToken : Token, ILessThanToken
    {
        public LessThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILessThanOrEqualTokenPlace : ITokenPlace { }
    public partial interface ILessThanOrEqualToken : IToken, ILessThanOrEqualTokenPlace { }
    internal partial class LessThanOrEqualToken : Token, ILessThanOrEqualToken
    {
        public LessThanOrEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IPlusEqualsTokenPlace : ITokenPlace { }
    public partial interface IPlusEqualsToken : IToken, IPlusEqualsTokenPlace { }
    internal partial class PlusEqualsToken : Token, IPlusEqualsToken
    {
        public PlusEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMinusEqualsTokenPlace : ITokenPlace { }
    public partial interface IMinusEqualsToken : IToken, IMinusEqualsTokenPlace { }
    internal partial class MinusEqualsToken : Token, IMinusEqualsToken
    {
        public MinusEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAsteriskEqualsTokenPlace : ITokenPlace { }
    public partial interface IAsteriskEqualsToken : IToken, IAsteriskEqualsTokenPlace { }
    internal partial class AsteriskEqualsToken : Token, IAsteriskEqualsToken
    {
        public AsteriskEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISlashEqualsTokenPlace : ITokenPlace { }
    public partial interface ISlashEqualsToken : IToken, ISlashEqualsTokenPlace { }
    internal partial class SlashEqualsToken : Token, ISlashEqualsToken
    {
        public SlashEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDollarTokenPlace : ITokenPlace { }
    public partial interface IDollarToken : IToken, IDollarTokenPlace { }
    internal partial class DollarToken : Token, IDollarToken
    {
        public DollarToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDollarLessThanTokenPlace : ITokenPlace { }
    public partial interface IDollarLessThanToken : IToken, IDollarLessThanTokenPlace { }
    internal partial class DollarLessThanToken : Token, IDollarLessThanToken
    {
        public DollarLessThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDollarLessThanNotEqualTokenPlace : ITokenPlace { }
    public partial interface IDollarLessThanNotEqualToken : IToken, IDollarLessThanNotEqualTokenPlace { }
    internal partial class DollarLessThanNotEqualToken : Token, IDollarLessThanNotEqualToken
    {
        public DollarLessThanNotEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDollarGreaterThanTokenPlace : ITokenPlace { }
    public partial interface IDollarGreaterThanToken : IToken, IDollarGreaterThanTokenPlace { }
    internal partial class DollarGreaterThanToken : Token, IDollarGreaterThanToken
    {
        public DollarGreaterThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDollarGreaterThanNotEqualTokenPlace : ITokenPlace { }
    public partial interface IDollarGreaterThanNotEqualToken : IToken, IDollarGreaterThanNotEqualTokenPlace { }
    internal partial class DollarGreaterThanNotEqualToken : Token, IDollarGreaterThanNotEqualToken
    {
        public DollarGreaterThanNotEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IQuestionTokenPlace : ITokenPlace { }
    public partial interface IQuestionToken : IToken, IQuestionTokenPlace { }
    internal partial class QuestionToken : Token, IQuestionToken
    {
        public QuestionToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IQuestionQuestionTokenPlace : ITokenPlace { }
    public partial interface IQuestionQuestionToken : IToken, IQuestionQuestionTokenPlace { }
    internal partial class QuestionQuestionToken : Token, IQuestionQuestionToken
    {
        public QuestionQuestionToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IQuestionDotTokenPlace : ITokenPlace { }
    public partial interface IQuestionDotToken : IToken, IQuestionDotTokenPlace { }
    internal partial class QuestionDotToken : Token, IQuestionDotToken
    {
        public QuestionDotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILessThanColonTokenPlace : ITokenPlace { }
    public partial interface ILessThanColonToken : IToken, ILessThanColonTokenPlace { }
    internal partial class LessThanColonToken : Token, ILessThanColonToken
    {
        public LessThanColonToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IEqualsGreaterThanTokenPlace : ITokenPlace { }
    public partial interface IEqualsGreaterThanToken : IToken, IEqualsGreaterThanTokenPlace { }
    internal partial class EqualsGreaterThanToken : Token, IEqualsGreaterThanToken
    {
        public EqualsGreaterThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMissingToken :
        IWhitespaceTokenPlace,
        ICommentTokenPlace,
        IUnexpectedTokenPlace,
        IOpenBraceTokenPlace,
        ICloseBraceTokenPlace,
        IOpenParenTokenPlace,
        ICloseParenTokenPlace,
        IOpenBracketTokenPlace,
        ICloseBracketTokenPlace,
        ISemicolonTokenPlace,
        ICommaTokenPlace,
        IPipeTokenPlace,
        IColonTokenPlace,
        IRightArrowTokenPlace,
        IHashTokenPlace,
        IHashHashTokenPlace,
        IDotTokenPlace,
        IDotDotTokenPlace,
        ILessThanDotDotTokenPlace,
        IDotDotLessThanTokenPlace,
        ILessThanDotDotLessThanTokenPlace,
        IAtSignTokenPlace,
        ICaretTokenPlace,
        ICaretDotTokenPlace,
        IPlusTokenPlace,
        IMinusTokenPlace,
        IAsteriskTokenPlace,
        ISlashTokenPlace,
        IEqualsTokenPlace,
        IEqualsEqualsTokenPlace,
        INotEqualTokenPlace,
        IGreaterThanTokenPlace,
        IGreaterThanOrEqualTokenPlace,
        ILessThanTokenPlace,
        ILessThanOrEqualTokenPlace,
        IPlusEqualsTokenPlace,
        IMinusEqualsTokenPlace,
        IAsteriskEqualsTokenPlace,
        ISlashEqualsTokenPlace,
        IDollarTokenPlace,
        IDollarLessThanTokenPlace,
        IDollarLessThanNotEqualTokenPlace,
        IDollarGreaterThanTokenPlace,
        IDollarGreaterThanNotEqualTokenPlace,
        IQuestionTokenPlace,
        IQuestionQuestionTokenPlace,
        IQuestionDotTokenPlace,
        ILessThanColonTokenPlace,
        IEqualsGreaterThanTokenPlace,
        ITokenPlace // Implied, but saves issues with commas
    { }
}
