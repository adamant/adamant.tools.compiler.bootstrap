using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class MutableTypeSyntax : TypeSyntax
    {
        public ExpressionSyntax ReferencedTypeExpression { get; }

        public MutableTypeSyntax(TextSpan span, ExpressionSyntax referencedTypeExpression)
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
