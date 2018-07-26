using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class ReturnStatementSyntax : StatementSyntax
    {
        public ExpressionSyntax Expression => Children.OfType<ExpressionSyntax>().SingleOrDefault();

        public ReturnStatementSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
