using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class ArgumentListSyntax : SyntaxBranchNode
    {
        public IReadOnlyList<ExpressionSyntax> Arguments { get; }

        public ArgumentListSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            Arguments = Children.OfType<ExpressionSyntax>().ToList().AsReadOnly();
        }
    }
}
