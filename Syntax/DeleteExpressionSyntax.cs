using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class DeleteExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Expression { get; }

        public DeleteExpressionSyntax(TextSpan span, [NotNull] ExpressionSyntax expression)
            : base(span)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            return $"delete {Expression}";
        }
    }
}
