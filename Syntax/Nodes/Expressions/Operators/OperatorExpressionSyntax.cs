using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators
{
    public class OperatorExpressionSyntax : ExpressionSyntax
    {
        public Token Operator { get; }

        public OperatorExpressionSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            Operator = Children.OfType<Token>().Single();
        }
    }
}
