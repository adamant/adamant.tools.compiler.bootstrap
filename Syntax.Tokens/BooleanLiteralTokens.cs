using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public interface IBooleanLiteralToken : IKeywordToken { }
    public abstract class BooleanLiteralToken : KeywordToken, IBooleanLiteralToken
    {
        public abstract bool Value { get; }

        protected BooleanLiteralToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface ITrueKeywordToken : IBooleanLiteralToken { }
    public class TrueKeywordToken : BooleanLiteralToken, ITrueKeywordToken
    {
        public override bool Value => true;

        public TrueKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }

    public interface IFalseKeywordToken : IBooleanLiteralToken { }
    public class FalseKeywordToken : BooleanLiteralToken, IFalseKeywordToken
    {
        public override bool Value => false;

        public FalseKeywordToken(TextSpan span)
            : base(span)
        {
        }
    }
}
