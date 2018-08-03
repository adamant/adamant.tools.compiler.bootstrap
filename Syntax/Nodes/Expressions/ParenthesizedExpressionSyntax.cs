using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Expression { get; }

        public ParenthesizedExpressionSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            Expression = Children.OfType<ExpressionSyntax>().Single();
        }
    }
}
