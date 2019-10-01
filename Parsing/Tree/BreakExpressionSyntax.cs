using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class BreakExpressionSyntax : ExpressionSyntax, IBreakExpressionSyntax
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
