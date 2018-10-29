using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public interface IDotToken : IOperatorToken { }
    public class DotToken : OperatorToken, IDotToken
    {
        public DotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IDotDotToken : IOperatorToken { }
    public class DotDotToken : OperatorToken, IDotDotToken
    {
        public DotDotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IAtSignToken : IOperatorToken { }
    public class AtSignToken : OperatorToken, IAtSignToken
    {
        public AtSignToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface ICaretToken : IOperatorToken { }
    public class CaretToken : OperatorToken, ICaretToken
    {
        public CaretToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface ICaretDotToken : IOperatorToken { }
    public class CaretDotToken : OperatorToken, ICaretDotToken
    {
        public CaretDotToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IPlusToken : IOperatorToken { }
    public class PlusToken : OperatorToken, IPlusToken
    {
        public PlusToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IMinusToken : IOperatorToken { }
    public class MinusToken : OperatorToken, IMinusToken
    {
        public MinusToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IAsteriskToken : IOperatorToken { }
    public class AsteriskToken : OperatorToken, IAsteriskToken
    {
        public AsteriskToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface ISlashToken : IOperatorToken { }
    public class SlashToken : OperatorToken, ISlashToken
    {
        public SlashToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IEqualsToken : IOperatorToken { }
    public class EqualsToken : OperatorToken, IEqualsToken
    {
        public EqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IEqualsEqualsToken : IOperatorToken { }
    public class EqualsEqualsToken : OperatorToken, IEqualsEqualsToken
    {
        public EqualsEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface INotEqualToken : IOperatorToken { }
    public class NotEqualToken : OperatorToken, INotEqualToken
    {
        public NotEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IGreaterThanToken : IOperatorToken { }
    public class GreaterThanToken : OperatorToken, IGreaterThanToken
    {
        public GreaterThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IGreaterThanOrEqualToken : IOperatorToken { }
    public class GreaterThanOrEqualToken : OperatorToken, IGreaterThanOrEqualToken
    {
        public GreaterThanOrEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface ILessThanToken : IOperatorToken { }
    public class LessThanToken : OperatorToken, ILessThanToken
    {
        public LessThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface ILessThanOrEqualToken : IOperatorToken { }
    public class LessThanOrEqualToken : OperatorToken, ILessThanOrEqualToken
    {
        public LessThanOrEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IPlusEqualsToken : IOperatorToken { }
    public class PlusEqualsToken : OperatorToken, IPlusEqualsToken
    {
        public PlusEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IMinusEqualsToken : IOperatorToken { }
    public class MinusEqualsToken : OperatorToken, IMinusEqualsToken
    {
        public MinusEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IAsteriskEqualsToken : IOperatorToken { }
    public class AsteriskEqualsToken : OperatorToken, IAsteriskEqualsToken
    {
        public AsteriskEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface ISlashEqualsToken : IOperatorToken { }
    public class SlashEqualsToken : OperatorToken, ISlashEqualsToken
    {
        public SlashEqualsToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IDollarToken : IOperatorToken { }
    public class DollarToken : OperatorToken, IDollarToken
    {
        public DollarToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IDollarLessThanToken : IOperatorToken { }
    public class DollarLessThanToken : OperatorToken, IDollarLessThanToken
    {
        public DollarLessThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IDollarLessThanNotEqualToken : IOperatorToken { }
    public class DollarLessThanNotEqualToken : OperatorToken, IDollarLessThanNotEqualToken
    {
        public DollarLessThanNotEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IDollarGreaterThanToken : IOperatorToken { }
    public class DollarGreaterThanToken : OperatorToken, IDollarGreaterThanToken
    {
        public DollarGreaterThanToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IDollarGreaterThanNotEqualToken : IOperatorToken { }
    public class DollarGreaterThanNotEqualToken : OperatorToken, IDollarGreaterThanNotEqualToken
    {
        public DollarGreaterThanNotEqualToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial class MissingToken :
        IDotToken,
        IDotDotToken,
        IAtSignToken,
        ICaretToken,
        ICaretDotToken,
        IPlusToken,
        IMinusToken,
        IAsteriskToken,
        ISlashToken,
        IEqualsToken,
        IEqualsEqualsToken,
        INotEqualToken,
        IGreaterThanToken,
        IGreaterThanOrEqualToken,
        ILessThanToken,
        ILessThanOrEqualToken,
        IPlusEqualsToken,
        IMinusEqualsToken,
        IAsteriskEqualsToken,
        ISlashEqualsToken,
        IDollarToken,
        IDollarLessThanToken,
        IDollarLessThanNotEqualToken,
        IDollarGreaterThanToken,
        IDollarGreaterThanNotEqualToken,
        IOperatorToken // Implied, but saves issues with commas
    { }
}
