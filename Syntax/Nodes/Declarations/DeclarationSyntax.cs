using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public abstract class DeclarationSyntax : SyntaxBranchNode
    {
        public abstract IdentifierToken Name { get; }

        protected DeclarationSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
