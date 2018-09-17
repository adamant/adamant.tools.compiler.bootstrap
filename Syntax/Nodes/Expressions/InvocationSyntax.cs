using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class InvocationSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Callee { get; set; }
        public ArgumentListSyntax ArgumentList { get; }
        public IReadOnlyList<ExpressionSyntax> Arguments { get; }

        public InvocationSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            Callee = Children.OfType<ExpressionSyntax>().First();
            ArgumentList = Children.OfType<ArgumentListSyntax>().Single();
            Arguments = ArgumentList.Arguments;
        }
    }
}
