using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    /// <summary>
    /// Tokens that don't generally need to be parsed
    /// </summary>
    public abstract class TriviaToken : Token
    {
        protected TriviaToken(TextSpan span)
            : base(span)
        {
        }
    }
}
