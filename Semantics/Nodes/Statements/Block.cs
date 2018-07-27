using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements
{
    public class Block : Statement
    {
        public new BlockSyntax Syntax { get; }
        public IReadOnlyList<Statement> Statements { get; }

        public Block(BlockSyntax syntax, IEnumerable<Statement> statements)
        {
            Syntax = syntax;
            Statements = statements.ToList().AsReadOnly();
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
