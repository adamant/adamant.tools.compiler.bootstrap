using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators
{
    public class BinaryOperatorExpressionSyntax : OperatorExpressionSyntax
    {
        public ExpressionSyntax LeftOperand { get; }

        public ExpressionSyntax RightOperand { get; }

        public BinaryOperatorExpressionSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            LeftOperand = Children.OfType<ExpressionSyntax>().First();

            RightOperand = Children.OfType<ExpressionSyntax>().Last();
        }
    }
}
