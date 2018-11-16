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
        public static IEndOfFileToken EndOfFile(TextSpan span)
        {
            return new EndOfFileToken(span);
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
        public static IColonColonToken ColonColon(TextSpan span)
        {
            return new ColonColonToken(span);
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

    public partial interface IWhitespaceToken : IToken { }
    internal partial class WhitespaceToken : Token, IWhitespaceToken
    {
        public WhitespaceToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICommentToken : IToken { }
    internal partial class CommentToken : Token, ICommentToken
    {
        public CommentToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IUnexpectedToken : IToken { }
    internal partial class UnexpectedToken : Token, IUnexpectedToken
    {
        public UnexpectedToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IEndOfFileToken : IToken { }
    internal partial class EndOfFileToken : Token, IEndOfFileToken
    {
        public EndOfFileToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOpenBraceToken : IToken { }
    internal partial class OpenBraceToken : Token, IOpenBraceToken
    {
        public OpenBraceToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICloseBraceToken : IToken { }
    internal partial class CloseBraceToken : Token, ICloseBraceToken
    {
        public CloseBraceToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOpenParenToken : IToken { }
    internal partial class OpenParenToken : Token, IOpenParenToken
    {
        public OpenParenToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICloseParenToken : IToken { }
    internal partial class CloseParenToken : Token, ICloseParenToken
    {
        public CloseParenToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOpenBracketToken : IToken { }
    internal partial class OpenBracketToken : Token, IOpenBracketToken
    {
        public OpenBracketToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICloseBracketToken : IToken { }
    internal partial class CloseBracketToken : Token, ICloseBracketToken
    {
        public CloseBracketToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISemicolonToken : IToken { }
    internal partial class SemicolonToken : Token, ISemicolonToken
    {
        public SemicolonToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICommaToken : IToken { }
    internal partial class CommaToken : Token, ICommaToken
    {
        public CommaToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IPipeToken : IToken { }
    internal partial class PipeToken : Token, IPipeToken
    {
        public PipeToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IColonToken : IToken { }
    internal partial class ColonToken : Token, IColonToken
    {
        public ColonToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IRightArrowToken : IToken { }
    internal partial class RightArrowToken : Token, IRightArrowToken
    {
        public RightArrowToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IHashToken : IToken { }
    internal partial class HashToken : Token, IHashToken
    {
        public HashToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IHashHashToken : IToken { }
    internal partial class HashHashToken : Token, IHashHashToken
    {
        public HashHashToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDotToken : IToken { }
    internal partial class DotToken : Token, IDotToken
    {
        public DotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IColonColonToken : IToken { }
    internal partial class ColonColonToken : Token, IColonColonToken
    {
        public ColonColonToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDotDotToken : IToken { }
    internal partial class DotDotToken : Token, IDotDotToken
    {
        public DotDotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILessThanDotDotToken : IToken { }
    internal partial class LessThanDotDotToken : Token, ILessThanDotDotToken
    {
        public LessThanDotDotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDotDotLessThanToken : IToken { }
    internal partial class DotDotLessThanToken : Token, IDotDotLessThanToken
    {
        public DotDotLessThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILessThanDotDotLessThanToken : IToken { }
    internal partial class LessThanDotDotLessThanToken : Token, ILessThanDotDotLessThanToken
    {
        public LessThanDotDotLessThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAtSignToken : IToken { }
    internal partial class AtSignToken : Token, IAtSignToken
    {
        public AtSignToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICaretToken : IToken { }
    internal partial class CaretToken : Token, ICaretToken
    {
        public CaretToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICaretDotToken : IToken { }
    internal partial class CaretDotToken : Token, ICaretDotToken
    {
        public CaretDotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IPlusToken : IToken { }
    internal partial class PlusToken : Token, IPlusToken
    {
        public PlusToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMinusToken : IToken { }
    internal partial class MinusToken : Token, IMinusToken
    {
        public MinusToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAsteriskToken : IToken { }
    internal partial class AsteriskToken : Token, IAsteriskToken
    {
        public AsteriskToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISlashToken : IToken { }
    internal partial class SlashToken : Token, ISlashToken
    {
        public SlashToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IEqualsToken : IToken { }
    internal partial class EqualsToken : Token, IEqualsToken
    {
        public EqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IEqualsEqualsToken : IToken { }
    internal partial class EqualsEqualsToken : Token, IEqualsEqualsToken
    {
        public EqualsEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INotEqualToken : IToken { }
    internal partial class NotEqualToken : Token, INotEqualToken
    {
        public NotEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IGreaterThanToken : IToken { }
    internal partial class GreaterThanToken : Token, IGreaterThanToken
    {
        public GreaterThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IGreaterThanOrEqualToken : IToken { }
    internal partial class GreaterThanOrEqualToken : Token, IGreaterThanOrEqualToken
    {
        public GreaterThanOrEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILessThanToken : IToken { }
    internal partial class LessThanToken : Token, ILessThanToken
    {
        public LessThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILessThanOrEqualToken : IToken { }
    internal partial class LessThanOrEqualToken : Token, ILessThanOrEqualToken
    {
        public LessThanOrEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IPlusEqualsToken : IToken { }
    internal partial class PlusEqualsToken : Token, IPlusEqualsToken
    {
        public PlusEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMinusEqualsToken : IToken { }
    internal partial class MinusEqualsToken : Token, IMinusEqualsToken
    {
        public MinusEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAsteriskEqualsToken : IToken { }
    internal partial class AsteriskEqualsToken : Token, IAsteriskEqualsToken
    {
        public AsteriskEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISlashEqualsToken : IToken { }
    internal partial class SlashEqualsToken : Token, ISlashEqualsToken
    {
        public SlashEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDollarToken : IToken { }
    internal partial class DollarToken : Token, IDollarToken
    {
        public DollarToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDollarLessThanToken : IToken { }
    internal partial class DollarLessThanToken : Token, IDollarLessThanToken
    {
        public DollarLessThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDollarLessThanNotEqualToken : IToken { }
    internal partial class DollarLessThanNotEqualToken : Token, IDollarLessThanNotEqualToken
    {
        public DollarLessThanNotEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDollarGreaterThanToken : IToken { }
    internal partial class DollarGreaterThanToken : Token, IDollarGreaterThanToken
    {
        public DollarGreaterThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDollarGreaterThanNotEqualToken : IToken { }
    internal partial class DollarGreaterThanNotEqualToken : Token, IDollarGreaterThanNotEqualToken
    {
        public DollarGreaterThanNotEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IQuestionToken : IToken { }
    internal partial class QuestionToken : Token, IQuestionToken
    {
        public QuestionToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IQuestionQuestionToken : IToken { }
    internal partial class QuestionQuestionToken : Token, IQuestionQuestionToken
    {
        public QuestionQuestionToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IQuestionDotToken : IToken { }
    internal partial class QuestionDotToken : Token, IQuestionDotToken
    {
        public QuestionDotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILessThanColonToken : IToken { }
    internal partial class LessThanColonToken : Token, ILessThanColonToken
    {
        public LessThanColonToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IEqualsGreaterThanToken : IToken { }
    internal partial class EqualsGreaterThanToken : Token, IEqualsGreaterThanToken
    {
        public EqualsGreaterThanToken(TextSpan span)
            : base(span)
        {
        }
    }
}
