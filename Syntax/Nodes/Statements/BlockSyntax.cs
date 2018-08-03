using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class BlockSyntax : StatementSyntax
    {
        public IEnumerable<StatementSyntax> Statements => Children.OfType<StatementSyntax>();

        public BlockSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }

        public IEnumerable<StatementSyntax> DecendantStatementsAndSelf()
        {
            var statements = new Stack<StatementSyntax>();
            statements.Push(this);
            while (statements.Any())
            {
                var statement = statements.Pop();
                yield return statement;
                if (statement is BlockSyntax block)
                {
                    // If we reversed the children, we would have a pre-order traversal,
                    // but this is more efficent
                    foreach (var child in block.Statements)
                        statements.Push(child);
                }
            }
        }
    }
}
