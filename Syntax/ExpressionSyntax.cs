using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    // An expression is also a statement
    public abstract class ExpressionSyntax : StatementSyntax
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
