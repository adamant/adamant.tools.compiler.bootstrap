using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements
{
    public class Block : Statement
    {
        public new BlockSyntax Syntax { get; }

        public Block(BlockSyntax syntax)
        {
            Syntax = syntax;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
