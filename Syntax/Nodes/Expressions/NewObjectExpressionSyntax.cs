using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class NewObjectExpressionSyntax : ExpressionSyntax
    {
        public TypeSyntax Type { get; }
        public ArgumentListSyntax ArgumentList { get; }
        public IReadOnlyList<ExpressionSyntax> Arguments { get; }

        public NewObjectExpressionSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            Type = Children.OfType<TypeSyntax>().Single();
            ArgumentList = Children.OfType<ArgumentListSyntax>().Single();
            Arguments = ArgumentList.Arguments;
        }
    }
}
