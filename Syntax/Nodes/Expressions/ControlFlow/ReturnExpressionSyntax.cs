using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow
{
    public class ReturnExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Expression { get; }

        public ReturnExpressionSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            Expression = Children.OfType<ExpressionSyntax>().SingleOrDefault();
        }
    }
}
