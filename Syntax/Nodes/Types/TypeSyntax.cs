using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Parse.Types
{
    public abstract class TypeSyntax : SyntaxBranchNode
    {
        protected TypeSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
