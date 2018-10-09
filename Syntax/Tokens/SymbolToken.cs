using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public abstract class SymbolToken : Token
    {
        protected SymbolToken(TextSpan span)
            : base(span)
        {
        }
    }
}
