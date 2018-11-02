using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public abstract class ExpressionSyntax : SyntaxNode
    {
        public TextSpan Span { get; }

        protected ExpressionSyntax(TextSpan span)
        {
            Span = span;
        }
    }
}
