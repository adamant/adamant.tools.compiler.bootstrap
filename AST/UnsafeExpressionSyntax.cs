using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class UnsafeExpressionSyntax : ExpressionSyntax, IUnsafeExpressionSyntax
    {
        private IExpressionSyntax expression;

        public ref IExpressionSyntax Expression => ref expression;

        public UnsafeExpressionSyntax(TextSpan span, IExpressionSyntax expression)
            : base(span)
        {
            this.expression = expression;
        }

        public override string ToString()
        {
            return $"unsafe ({Expression})";
        }
    }
}
