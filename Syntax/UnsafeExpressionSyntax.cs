using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class UnsafeExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Expression { get; }

        public UnsafeExpressionSyntax(TextSpan span, [NotNull] ExpressionSyntax expression)
            : base(span)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            return $"unsafe ({Expression})";
        }
    }
}
