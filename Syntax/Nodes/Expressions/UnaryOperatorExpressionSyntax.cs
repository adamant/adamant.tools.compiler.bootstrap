using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class UnaryOperatorExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Operand => Children.OfType<ExpressionSyntax>().Single();

        public UnaryOperatorExpressionSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
