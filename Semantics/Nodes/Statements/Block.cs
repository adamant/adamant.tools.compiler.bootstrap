using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements
{
    public class Block : Statement
    {
        public new BlockSyntax Syntax { get; }
        public IReadOnlyList<Statement> Statements { get; }

        public Block(BlockSyntax syntax)
        {
            Syntax = syntax;
            Statements = new List<Statement>();
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
