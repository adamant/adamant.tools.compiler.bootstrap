using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class ReturnStatementSyntax : StatementSyntax
    {
        public ReturnStatementSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
