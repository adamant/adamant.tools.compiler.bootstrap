using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public abstract class ExpressionSyntax : NonTerminal
    {
        public TextSpan Span { get; }

        protected ExpressionSyntax(TextSpan span)
        {
            Span = span;
        }

        // Useful for debugging
        public abstract override string ToString();
    }
}
