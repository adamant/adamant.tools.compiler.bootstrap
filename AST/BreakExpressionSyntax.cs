using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class BreakExpressionSyntax : ExpressionSyntax, IBreakExpressionSyntax
    {
        private IExpressionSyntax? value;
        public ref IExpressionSyntax? Value => ref value;

        public BreakExpressionSyntax(
            TextSpan span,
            IExpressionSyntax value)
            : base(span)
        {
            this.value = value;
        }

        public override string ToString()
        {
            if (Value != null)
                return $"break {Value}";
            return "break";
        }
    }
}
