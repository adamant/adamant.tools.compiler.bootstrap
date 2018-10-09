using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public abstract class OperatorToken : Token
    {
        protected OperatorToken(TextSpan span)
            : base(span)
        {
        }
    }
}
