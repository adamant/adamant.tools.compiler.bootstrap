using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface ISymbolToken : IToken { }
    public abstract class SymbolToken : Token, ISymbolToken
    {
        protected SymbolToken(TextSpan span)
            : base(span)
        {
        }
    }
}
