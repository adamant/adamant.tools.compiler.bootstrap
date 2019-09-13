using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class UnsafeExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Expression { get; }

        public UnsafeExpressionSyntax(TextSpan span, ExpressionSyntax expression)
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
