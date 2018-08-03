using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators
{
    public class UnaryOperatorExpressionSyntax : OperatorExpressionSyntax
    {
        public ExpressionSyntax Operand { get; }

        public UnaryOperatorExpressionSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            Operand = Children.OfType<ExpressionSyntax>().Single();
        }
    }
}
