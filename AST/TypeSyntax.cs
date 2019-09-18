using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        //typeof(RefTypeSyntax),
        typeof(ReferenceLifetimeSyntax),
        typeof(SelfTypeExpressionSyntax),
        typeof(NameSyntax),
        typeof(MutableExpressionSyntax))]
    public abstract class TypeSyntax : ExpressionSyntax
    {
        protected TypeSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
