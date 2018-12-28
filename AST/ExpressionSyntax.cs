using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    // An expression is also a statement
    public abstract class ExpressionSyntax : StatementSyntax
    {
        public TextSpan Span { get; }
        public ValueSemantics? ValueSemantics { get; set; } = null;

        protected ExpressionSyntax(TextSpan span)
        {
            Span = span;
        }

        // Useful for debugging
        public abstract override string ToString();
    }
}
