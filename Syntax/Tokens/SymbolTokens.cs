using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public interface IOpenBraceToken : ISymbolToken { }
    public class OpenBraceToken : SymbolToken, IOpenBraceToken
    {
        public OpenBraceToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface ICloseBraceToken : ISymbolToken { }
    public class CloseBraceToken : SymbolToken, ICloseBraceToken
    {
        public CloseBraceToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IOpenParenToken : ISymbolToken { }
    public class OpenParenToken : SymbolToken, IOpenParenToken
    {
        public OpenParenToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface ICloseParenToken : ISymbolToken { }
    public class CloseParenToken : SymbolToken, ICloseParenToken
    {
        public CloseParenToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IOpenBracketToken : ISymbolToken { }
    public class OpenBracketToken : SymbolToken, IOpenBracketToken
    {
        public OpenBracketToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface ICloseBracketToken : ISymbolToken { }
    public class CloseBracketToken : SymbolToken, ICloseBracketToken
    {
        public CloseBracketToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface ISemicolonToken : ISymbolToken { }
    public class SemicolonToken : SymbolToken, ISemicolonToken
    {
        public SemicolonToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface ICommaToken : ISymbolToken { }
    public class CommaToken : SymbolToken, ICommaToken
    {
        public CommaToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IPipeToken : ISymbolToken { }
    public class PipeToken : SymbolToken, IPipeToken
    {
        public PipeToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IQuestionToken : ISymbolToken { }
    public class QuestionToken : SymbolToken, IQuestionToken
    {
        public QuestionToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IColonToken : ISymbolToken { }
    public class ColonToken : SymbolToken, IColonToken
    {
        public ColonToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IRightArrowToken : ISymbolToken { }
    public class RightArrowToken : SymbolToken, IRightArrowToken
    {
        public RightArrowToken(TextSpan span)
            : base(span)
        {
        }
    }

    public partial class MissingToken :
        IOpenBraceToken,
        ICloseBraceToken,
        IOpenParenToken,
        ICloseParenToken,
        IOpenBracketToken,
        ICloseBracketToken,
        ISemicolonToken,
        ICommaToken,
        IPipeToken,
        IQuestionToken,
        IColonToken,
        IRightArrowToken,
        ISymbolToken // Implied, but saves issues with commas
    { }
}
