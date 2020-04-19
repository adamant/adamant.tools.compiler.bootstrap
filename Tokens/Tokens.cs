using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public static partial class TokenFactory
    {
        public static IEndOfFileToken EndOfFile(TextSpan span)
        {
            return new EndOfFileToken(span);
        }

        public static IOpenBraceToken OpenBrace(TextSpan span)
        {
            return new OpenBraceToken(span);
        }

        public static ICloseBraceToken CloseBrace(TextSpan span)
        {
            return new CloseBraceToken(span);
        }

        public static IOpenParenToken OpenParen(TextSpan span)
        {
            return new OpenParenToken(span);
        }

        public static ICloseParenToken CloseParen(TextSpan span)
        {
            return new CloseParenToken(span);
        }

        public static ISemicolonToken Semicolon(TextSpan span)
        {
            return new SemicolonToken(span);
        }

        public static ICommaToken Comma(TextSpan span)
        {
            return new CommaToken(span);
        }

        public static IColonToken Colon(TextSpan span)
        {
            return new ColonToken(span);
        }

        public static IRightArrowToken RightArrow(TextSpan span)
        {
            return new RightArrowToken(span);
        }

        public static IDotToken Dot(TextSpan span)
        {
            return new DotToken(span);
        }

        public static IColonColonDotToken ColonColonDot(TextSpan span)
        {
            return new ColonColonDotToken(span);
        }

        public static IDotDotToken DotDot(TextSpan span)
        {
            return new DotDotToken(span);
        }

        public static ILessThanDotDotToken LessThanDotDot(TextSpan span)
        {
            return new LessThanDotDotToken(span);
        }

        public static IDotDotLessThanToken DotDotLessThan(TextSpan span)
        {
            return new DotDotLessThanToken(span);
        }

        public static ILessThanDotDotLessThanToken LessThanDotDotLessThan(TextSpan span)
        {
            return new LessThanDotDotLessThanToken(span);
        }

        public static IPlusToken Plus(TextSpan span)
        {
            return new PlusToken(span);
        }

        public static IMinusToken Minus(TextSpan span)
        {
            return new MinusToken(span);
        }

        public static IAsteriskToken Asterisk(TextSpan span)
        {
            return new AsteriskToken(span);
        }

        public static ISlashToken Slash(TextSpan span)
        {
            return new SlashToken(span);
        }

        public static IEqualsToken Equals(TextSpan span)
        {
            return new EqualsToken(span);
        }

        public static IEqualsEqualsToken EqualsEquals(TextSpan span)
        {
            return new EqualsEqualsToken(span);
        }

        public static INotEqualToken NotEqual(TextSpan span)
        {
            return new NotEqualToken(span);
        }

        public static IGreaterThanToken GreaterThan(TextSpan span)
        {
            return new GreaterThanToken(span);
        }

        public static IGreaterThanOrEqualToken GreaterThanOrEqual(TextSpan span)
        {
            return new GreaterThanOrEqualToken(span);
        }

        public static ILessThanToken LessThan(TextSpan span)
        {
            return new LessThanToken(span);
        }

        public static ILessThanOrEqualToken LessThanOrEqual(TextSpan span)
        {
            return new LessThanOrEqualToken(span);
        }

        public static IPlusEqualsToken PlusEquals(TextSpan span)
        {
            return new PlusEqualsToken(span);
        }

        public static IMinusEqualsToken MinusEquals(TextSpan span)
        {
            return new MinusEqualsToken(span);
        }

        public static IAsteriskEqualsToken AsteriskEquals(TextSpan span)
        {
            return new AsteriskEqualsToken(span);
        }

        public static ISlashEqualsToken SlashEquals(TextSpan span)
        {
            return new SlashEqualsToken(span);
        }

        public static IQuestionToken Question(TextSpan span)
        {
            return new QuestionToken(span);
        }

        public static IQuestionQuestionToken QuestionQuestion(TextSpan span)
        {
            return new QuestionQuestionToken(span);
        }

        public static IQuestionDotToken QuestionDot(TextSpan span)
        {
            return new QuestionDotToken(span);
        }

        public static ILessThanColonToken LessThanColon(TextSpan span)
        {
            return new LessThanColonToken(span);
        }

        public static IEqualsGreaterThanToken EqualsGreaterThan(TextSpan span)
        {
            return new EqualsGreaterThanToken(span);
        }

    }

    [Closed(
        typeof(IEndOfFileToken),
        typeof(IOpenBraceToken),
        typeof(ICloseBraceToken),
        typeof(IOpenParenToken),
        typeof(ICloseParenToken),
        typeof(ISemicolonToken),
        typeof(ICommaToken),
        typeof(IColonToken),
        typeof(IRightArrowToken),
        typeof(IDotToken),
        typeof(IColonColonDotToken),
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
        typeof(IEqualsGreaterThanToken))]
    public partial interface IEssentialToken { }


    public partial interface IEndOfFileToken : IEssentialToken { }
    internal partial class EndOfFileToken : Token, IEndOfFileToken
    {
        public EndOfFileToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOpenBraceToken : IEssentialToken { }
    internal partial class OpenBraceToken : Token, IOpenBraceToken
    {
        public OpenBraceToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICloseBraceToken : IEssentialToken { }
    internal partial class CloseBraceToken : Token, ICloseBraceToken
    {
        public CloseBraceToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IOpenParenToken : IEssentialToken { }
    internal partial class OpenParenToken : Token, IOpenParenToken
    {
        public OpenParenToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICloseParenToken : IEssentialToken { }
    internal partial class CloseParenToken : Token, ICloseParenToken
    {
        public CloseParenToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISemicolonToken : IEssentialToken { }
    internal partial class SemicolonToken : Token, ISemicolonToken
    {
        public SemicolonToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ICommaToken : IEssentialToken { }
    internal partial class CommaToken : Token, ICommaToken
    {
        public CommaToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IColonToken : IEssentialToken { }
    internal partial class ColonToken : Token, IColonToken
    {
        public ColonToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IRightArrowToken : IEssentialToken { }
    internal partial class RightArrowToken : Token, IRightArrowToken
    {
        public RightArrowToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDotToken : IEssentialToken { }
    internal partial class DotToken : Token, IDotToken
    {
        public DotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IColonColonDotToken : IEssentialToken { }
    internal partial class ColonColonDotToken : Token, IColonColonDotToken
    {
        public ColonColonDotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDotDotToken : IEssentialToken { }
    internal partial class DotDotToken : Token, IDotDotToken
    {
        public DotDotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILessThanDotDotToken : IEssentialToken { }
    internal partial class LessThanDotDotToken : Token, ILessThanDotDotToken
    {
        public LessThanDotDotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IDotDotLessThanToken : IEssentialToken { }
    internal partial class DotDotLessThanToken : Token, IDotDotLessThanToken
    {
        public DotDotLessThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILessThanDotDotLessThanToken : IEssentialToken { }
    internal partial class LessThanDotDotLessThanToken : Token, ILessThanDotDotLessThanToken
    {
        public LessThanDotDotLessThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IPlusToken : IEssentialToken { }
    internal partial class PlusToken : Token, IPlusToken
    {
        public PlusToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMinusToken : IEssentialToken { }
    internal partial class MinusToken : Token, IMinusToken
    {
        public MinusToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAsteriskToken : IEssentialToken { }
    internal partial class AsteriskToken : Token, IAsteriskToken
    {
        public AsteriskToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISlashToken : IEssentialToken { }
    internal partial class SlashToken : Token, ISlashToken
    {
        public SlashToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IEqualsToken : IEssentialToken { }
    internal partial class EqualsToken : Token, IEqualsToken
    {
        public EqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IEqualsEqualsToken : IEssentialToken { }
    internal partial class EqualsEqualsToken : Token, IEqualsEqualsToken
    {
        public EqualsEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface INotEqualToken : IEssentialToken { }
    internal partial class NotEqualToken : Token, INotEqualToken
    {
        public NotEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IGreaterThanToken : IEssentialToken { }
    internal partial class GreaterThanToken : Token, IGreaterThanToken
    {
        public GreaterThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IGreaterThanOrEqualToken : IEssentialToken { }
    internal partial class GreaterThanOrEqualToken : Token, IGreaterThanOrEqualToken
    {
        public GreaterThanOrEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILessThanToken : IEssentialToken { }
    internal partial class LessThanToken : Token, ILessThanToken
    {
        public LessThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILessThanOrEqualToken : IEssentialToken { }
    internal partial class LessThanOrEqualToken : Token, ILessThanOrEqualToken
    {
        public LessThanOrEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IPlusEqualsToken : IEssentialToken { }
    internal partial class PlusEqualsToken : Token, IPlusEqualsToken
    {
        public PlusEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IMinusEqualsToken : IEssentialToken { }
    internal partial class MinusEqualsToken : Token, IMinusEqualsToken
    {
        public MinusEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IAsteriskEqualsToken : IEssentialToken { }
    internal partial class AsteriskEqualsToken : Token, IAsteriskEqualsToken
    {
        public AsteriskEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ISlashEqualsToken : IEssentialToken { }
    internal partial class SlashEqualsToken : Token, ISlashEqualsToken
    {
        public SlashEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IQuestionToken : IEssentialToken { }
    internal partial class QuestionToken : Token, IQuestionToken
    {
        public QuestionToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IQuestionQuestionToken : IEssentialToken { }
    internal partial class QuestionQuestionToken : Token, IQuestionQuestionToken
    {
        public QuestionQuestionToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IQuestionDotToken : IEssentialToken { }
    internal partial class QuestionDotToken : Token, IQuestionDotToken
    {
        public QuestionDotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface ILessThanColonToken : IEssentialToken { }
    internal partial class LessThanColonToken : Token, ILessThanColonToken
    {
        public LessThanColonToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial interface IEqualsGreaterThanToken : IEssentialToken { }
    internal partial class EqualsGreaterThanToken : Token, IEqualsGreaterThanToken
    {
        public EqualsGreaterThanToken(TextSpan span)
            : base(span)
        {
        }
    }
}
