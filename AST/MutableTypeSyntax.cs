using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class MutableTypeSyntax : TypeSyntax
    {
        [NotNull] public ExpressionSyntax ReferencedTypeExpression { get; }

        public MutableTypeSyntax(TextSpan span, [NotNull] ExpressionSyntax referencedTypeExpression)
            : base(span)
        {
            ReferencedTypeExpression = referencedTypeExpression;
        }

        public override string ToString()
        {
            return $"ref {ReferencedTypeExpression}";
        }
    }
}
