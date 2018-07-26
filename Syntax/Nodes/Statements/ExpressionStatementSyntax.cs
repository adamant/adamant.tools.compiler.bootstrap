using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class ExpressionStatementSyntax : StatementSyntax
    {
        public ExpressionSyntax Expression => Children.OfType<ExpressionSyntax>().Single();

        public ExpressionStatementSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
