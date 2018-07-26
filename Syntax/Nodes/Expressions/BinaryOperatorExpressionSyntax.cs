using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class BinaryOperatorExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax LeftOperand => Children.OfType<ExpressionSyntax>().First();
        public Token Operator => Children.OfType<Token>().Single();
        public ExpressionSyntax RightOperand => Children.OfType<ExpressionSyntax>().Last();

        public BinaryOperatorExpressionSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
