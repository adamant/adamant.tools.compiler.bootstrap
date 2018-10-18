using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public interface IOperatorToken : IToken { }
    public abstract class OperatorToken : Token, IOperatorToken
    {
        protected OperatorToken(TextSpan span)
            : base(span)
        {
        }
    }
}
